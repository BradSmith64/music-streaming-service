using System.Text.RegularExpressions;
using music_streaming_domain.Songs;

namespace music_streaming_application;

public class RegisterSongUseCase
{
    private readonly ISongRepository _repository;
    private readonly ISongStorage _storage;
    private readonly IMetadataService _metadataService;

    public RegisterSongUseCase(ISongRepository repository, ISongStorage storage, IMetadataService metadataService)
    {
        _repository = repository;
        _storage = storage;
        _metadataService = metadataService;
    }

    public async Task ExecuteAsync(RegisterSongCommand command)
    {
        // 1. Get Metadata (Reading just a small part of the stream)
        using var metadataStream = await _storage.OpenReadStreamAsync(command.BlobUri);
        var metadata = await _metadataService.ExtractMetadataAsync(metadataStream);

        // 2. Idempotency Check: Check if song already exists in database
        var existingSong = await _repository.GetSongByTitleAndAlbumAsync(metadata.Title, metadata.AlbumTitle);
        if (existingSong != null)
        {
            return;
        }

        // 3. Ensure Artist Exists
        var artistName = metadata.Artist ?? "Unknown Artist";
        var artist = await _repository.GetArtistByNameAsync(artistName);
        if (artist == null)
        {
            artist = new Artist { Id = 0, Name = artistName };
            artist.Id = await _repository.AddArtistAsync(artist);
        }

        // 4. Ensure Album Exists
        var album = await _repository.GetAlbumByTitleAndArtistAsync(metadata.AlbumTitle, artist.Id);
        if (album == null)
        {
            album = new Album { Id = 0, Title = metadata.AlbumTitle, Artist = artist };
            album.Id = await _repository.AddAlbumAsync(album);
        }

        // 5. Generate a Deterministic Filename
        var extension = Path.GetExtension(command.BlobUri);
        var safeFileName = GenerateDeterministicFileName(command.UploaderId, metadata.AlbumTitle, metadata.Title, extension);

        // 6. Persist Media to permanent storage (Dumb Overwrite)
        using var uploadStream = await _storage.OpenReadStreamAsync(command.BlobUri);
        await _storage.UploadFileAsync(safeFileName, uploadStream);

        // 7. Persist Metadata to database
        var song = new Song
        {
            Id = 0,
            Title = metadata.Title,
            Album = album,
            ReleaseDate = metadata.ReleaseDate,
            FileName = safeFileName,
            Likes = new List<Like>()
        };

        await _repository.AddSongAsync(song);

        // 8. Cleanup the audio file from landing zone
        await _storage.DeleteFileAsync(command.BlobUri);
    }

    private string GenerateDeterministicFileName(string userId, string album, string title, string extension)
    {
        string Slugify(string text) => 
            Regex.Replace(text.ToLowerInvariant(), @"[^a-z0-9]", "-").Trim('-');

        return $"{Slugify(userId)}-{Slugify(album)}-{Slugify(title)}{extension}";
    }
}

public class RegisterSongCommand
{
    public required string BlobUri { get; set; }
    public required string UploaderId { get; set; }
}