using music_streaming_domain.Songs;

namespace music_streaming_application;

public interface ISongRepository
{
    public Task<Song> GetSongByIdAsync(int songId);
    public Task<Song?> GetSongByTitleAndAlbumAsync(string title, string albumTitle);
    public Task<int> AddSongAsync(Song song);

    public Task<Artist?> GetArtistByNameAsync(string name);
    public Task<int> AddArtistAsync(Artist artist);

    public Task<Album?> GetAlbumByTitleAndArtistAsync(string title, int artistId);
    public Task<int> AddAlbumAsync(Album album);

    // As likes are a list and not a scalar property, it's best to have a dedicated method
    // here rather than a "generic" update function.
    public Task<int> LikeSongAsync(Song song, Like like);
    public Task UnlikeSongAsync(Song song, int userId);
}