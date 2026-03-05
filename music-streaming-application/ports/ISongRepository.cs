using music_streaming_domain.Songs;

namespace music_streaming_application;

public interface ISongRepository
{
    public Task<Song> GetSongByIdAsync(int songId);

    // As likes are a list and not a scalar property, it's best to have a dedicated method
    // here rather than a "generic" update function.
    public Task<int> LikeSongAsync(Song song, Like like);
    public Task UnlikeSongAsync(Song song, int userId);
}