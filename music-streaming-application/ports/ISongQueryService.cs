using music_streaming_domain;

namespace music_streaming_application;

public interface ISongQueryService
{
    public Task<List<SongMetadata>> GetSongsAsync(int userId);
}