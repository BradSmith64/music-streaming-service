using music_streaming_domain.Songs;

namespace music_streaming_infrastructure;

public static class MockSongDatabase
{
    private static Persistence.Artist _mockArtist = new Persistence.Artist { Id = 1, Name = "Mock Artist" };
    private static Persistence.Album _mockAlbum = new Persistence.Album { Id = 1, Title = "Mock Album", ArtistId = 1, Artist = _mockArtist };

    private static List<Persistence.Song> _songs = new List<Persistence.Song>()
    {
        new Persistence.Song { Id = 1, Title = "Song 1", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 2, Title = "Song 2", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 3, Title = "Song 3", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 4, Title = "Song 4", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 5, Title = "Song 5", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 6, Title = "Song 6", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 7, Title = "Song 7", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 8, Title = "Song 8", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample2.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 9, Title = "Song 9", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample3.wav", Likes = new List<Persistence.Like>() },
        new Persistence.Song { Id = 10, Title = "Song 10", AlbumId = 1, Album = _mockAlbum, ReleaseDate = DateTime.UtcNow, FileName = "sample1.mp3", Likes = new List<Persistence.Like>() }
    };

    public async static Task<List<Persistence.Song>> GetSongs()
    {
        return await Task.FromResult(_songs);
    }

    public async static Task<Persistence.Song?> GetSongById(int id)
    {
        var entity = _songs.FirstOrDefault(song => song.Id == id);
        return entity;
    }
}