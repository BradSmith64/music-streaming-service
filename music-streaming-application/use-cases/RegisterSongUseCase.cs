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
        Stream? metadataStream = null;
        ID3Metadata metadata;
        try
        {
            metadataStream = await _storage.OpenLandingZoneStreamAsync(command.BlobUri);
            metadata = await _metadataService.ExtractMetadataAsync(metadataStream, command.BlobUri);
        }
        catch (Exception ex)
        {
            throw new MetadataExtractionException(command.BlobUri, "Failed to open or read stream for metadata extraction", ex);
        }
        finally
        {
            metadataStream?.Dispose();
        }

        if (string.IsNullOrWhiteSpace(metadata.Title))
        {
            throw new MetadataExtractionException(command.BlobUri, "Extracted title is empty or null.");
        }

        // 2. Ensure Artist Exists
        var artistName = metadata.Artist ?? "Unknown Artist";
        Artist? artist;
        try 
        {
            artist = await _repository.GetArtistByNameAsync(artistName);
            if (artist == null)
            {
                artist = new Artist { Id = 0, Name = artistName };
                artist.Id = await _repository.AddArtistAsync(artist);
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to resolve or create artist: {artistName}", ex);
        }

        // 3. Ensure Album Exists
        var albumTitle = metadata.AlbumTitle ?? "Unknown Album";
        Album? album;
        try
        {
            album = await _repository.GetAlbumByTitleAndArtistAsync(albumTitle, artist.Id);
            if (album == null)
            {
                album = new Album { Id = 0, Title = albumTitle, Artist = artist };
                album.Id = await _repository.AddAlbumAsync(album);
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to resolve or create album: {albumTitle}", ex);
        }

        // 4. Idempotency Check: Check if song already exists in this artist/album context
        var existingSong = await _repository.GetSongByTitleAlbumAndArtistAsync(metadata.Title, albumTitle, artist.Id);
        if (existingSong != null)
        {
            throw new SongAlreadyRegisteredException(metadata.Title, albumTitle);
        }

        // 5. Generate a Deterministic Filename
        var extension = Path.GetExtension(command.BlobUri);
        var safeFileName = GenerateDeterministicFileName(command.UploaderId, albumTitle, metadata.Title, extension);

        // 6. Persist Media to permanent storage
        try
        {
            using var uploadStream = await _storage.OpenLandingZoneStreamAsync(command.BlobUri);
            await _storage.PersistToPermanentStorageAsync(safeFileName, uploadStream);
        }
        catch (Exception ex)
        {
            throw new StorageOperationException("PersistToPermanentStorage", command.BlobUri, ex);
        }

        // 7. Persist Metadata to database
        try
        {
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
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to persist song metadata for '{metadata.Title}'", ex);
        }

        // 8. Cleanup the audio file from landing zone
        try
        {
            await _storage.PurgeFromLandingZoneAsync(command.BlobUri);
        }
        catch (Exception ex)
        {
            // We might not want to fail the whole operation if cleanup fails, 
            // but for maximum visibility we'll throw and let it retry or DLQ.
            throw new StorageOperationException("PurgeFromLandingZone", command.BlobUri, ex);
        }
    }

    private string GenerateDeterministicFileName(string userId, string album, string title, string extension)
    {
        string Slugify(string text)
        {
            // Remove null characters and other non-printable control characters
            var cleanText = new string(text.Where(c => !char.IsControl(c) && c != '\0').ToArray());
            return Regex.Replace(cleanText.ToLowerInvariant(), @"[^a-z0-9]", "-").Trim('-');
        }

        return $"{Slugify(userId)}-{Slugify(album)}-{Slugify(title)}{extension}";
    }
}

public class RegisterSongCommand
{
    public required string BlobUri { get; set; }
    public required string UploaderId { get; set; }
}
