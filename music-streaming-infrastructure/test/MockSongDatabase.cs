using music_streaming_domain.Songs;

namespace music_streaming_infrastructure;

public static class MockSongDatabase
{
    private static List<Persistence.Song> _songs = new List<Persistence.Song>()
    {
        new Persistence.Song { Id = 1, Title = "Song 1", AlbumTitle = "Album 1", ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 2, Title = "Song 2", AlbumTitle = "Album 2", ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 3, Title = "Song 3", AlbumTitle = "Album 3", ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 4, Title = "Song 4", AlbumTitle = "Album 4", ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 5, Title = "Song 5", AlbumTitle = "Album 5", ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 6, Title = "Song 6", AlbumTitle = "Album 6", ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 7, Title = "Song 7", AlbumTitle = "Album 7", ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 8, Title = "Song 8", AlbumTitle = "Album 8", ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 9, Title = "Song 9", AlbumTitle = "Album 9", ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 10, Title = "Song 10", AlbumTitle = "Album 10", ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 11, Title = "Song 11", AlbumTitle = "Album 11", ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 12, Title = "Song 12", AlbumTitle = "Album 12", ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 13, Title = "Song 13", AlbumTitle = "Album 13", ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 14, Title = "Song 14", AlbumTitle = "Album 14", ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 15, Title = "Song 15", AlbumTitle = "Album 15", ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 16, Title = "Song 16", AlbumTitle = "Album 16", ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 17, Title = "Song 17", AlbumTitle = "Album 17", ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 18, Title = "Song 18", AlbumTitle = "Album 18", ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 19, Title = "Song 19", AlbumTitle = "Album 19", ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 20, Title = "Song 20", AlbumTitle = "Album 20", ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() }
    };

    public async static Task<List<Persistence.Song>> GetSongs()
    {
        return await Task.FromResult(_songs);
    }

    public async static Task<Persistence.Song?> GetSongById(int id)
    {
        var entity = _songs.FirstOrDefault(song => song.Id == id);

        if( entity == null )
        {
            return null;
        }

        return entity;
    }
}