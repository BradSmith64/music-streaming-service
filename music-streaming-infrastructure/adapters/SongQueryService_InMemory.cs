using music_streaming_application;
using music_streaming_domain;
using music_streaming_domain.Songs;

namespace music_streaming_infrastructure;

public class SongQueryService_InMemory : ISongQueryService
{
    public async Task<List<SongMetadata>> GetSongsAsync(int userId)
    {
        var songs = await MockSongDatabase.GetSongs();
        return songs.Select( entity =>
            new SongMetadata { SongId = entity.Id, Title = entity.Title, AlbumTitle = entity.AlbumTitle, ReleaseDate = entity.ReleaseDate, FileName = entity.FileName, Url = "http://localhost:8080/" + entity.FileName, LikeCount = entity.Likes.Count, LikedByUser = entity.Likes.Any( ( like ) => like.UserId == userId ) })
        .ToList();
    }
}