using music_streaming_application;
using music_streaming_domain.Songs;
using music_streaming_infrastructure;

public class SongRepository_InMemory : ISongRepository
{
    private readonly object _lock = new object();

    public async Task<Song> GetSongByIdAsync(int songId)
    {
        var entity = await MockSongDatabase.GetSongById(songId);
        if (entity == null) throw new SongNotFoundException(songId);

        return MapToDomain(entity);
    }

    public Task<Song?> GetSongByTitleAndAlbumAsync(string title, string albumTitle)
    {
        // Simple mock implementation
        return Task.FromResult<Song?>(null);
    }

    public Task<int> AddSongAsync(Song song)
    {
        return Task.FromResult(0);
    }

    public Task<Artist?> GetArtistByNameAsync(string name)
    {
        return Task.FromResult<Artist?>(null);
    }

    public Task<int> AddArtistAsync(Artist artist)
    {
        return Task.FromResult(0);
    }

    public Task<Album?> GetAlbumByTitleAndArtistAsync(string title, int artistId)
    {
        return Task.FromResult<Album?>(null);
    }

    public Task<int> AddAlbumAsync(Album album)
    {
        return Task.FromResult(0);
    }

    public async Task<int> LikeSongAsync(Song song, Like like)
    {
        var entity = await MockSongDatabase.GetSongById(song.Id);
        if (entity == null) throw new SongNotFoundException(song.Id);

        lock (_lock)
        {
            int newId = entity.Likes.Any() ? entity.Likes.Max(l => l.Id) + 1 : 1;
            entity.Likes.Add(new music_streaming_infrastructure.Persistence.Like 
            { 
                Id = newId, 
                SongId = song.Id, 
                UserId = like.UserId, 
                CreatedAt = DateTime.UtcNow 
            });
            return newId;
        }
    }

    public async Task UnlikeSongAsync(Song song, int userId)
    {
        var entity = await MockSongDatabase.GetSongById(song.Id);
        if (entity == null) throw new SongNotFoundException(song.Id);

        var like = entity.Likes.FirstOrDefault(l => l.UserId == userId);
        if (like != null)
        {
            entity.Likes.Remove(like);
        }
    }

    private Song MapToDomain(music_streaming_infrastructure.Persistence.Song entity)
    {
        // Note: MockSongDatabase is still using old structure, so we mock the Album/Artist part
        return new Song
        {
            Id = entity.Id,
            Title = entity.Title,
            Album = new Album { Id = 0, Title = "Mock Album", Artist = new Artist { Id = 0, Name = "Mock Artist" } },
            ReleaseDate = entity.ReleaseDate,
            FileName = entity.FileName,
            Likes = entity.Likes.Select(l => new Like { Id = l.Id, SongId = l.SongId, UserId = l.UserId, CreatedAt = l.CreatedAt }).ToList()
        };
    }
}