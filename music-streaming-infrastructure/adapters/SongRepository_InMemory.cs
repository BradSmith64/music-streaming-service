using music_streaming_application;
using music_streaming_domain.Songs;
using music_streaming_infrastructure;

public class SongRepository_InMemory : ISongRepository
{
    // Assumes that node http-server is running on port 8080 for serving files.

    private readonly object _lock = new object();

    public async Task<Song> GetSongByIdAsync(int songId)
    {
        var entity = await MockSongDatabase.GetSongById(songId);

        if( entity == null )
        {
            throw new SongNotFoundException(songId);
        }

        return await Task.FromResult(new Song()
        {
            Id = entity.Id,
            Title = entity.Title,
            AlbumTitle = entity.AlbumTitle,
            ReleaseDate = entity.ReleaseDate,
            FileName = entity.FileName,
            Likes = entity.Likes.Select( ( like ) => new Like { Id = like.Id, SongId = like.SongId, UserId = like.UserId, CreatedAt = like.CreatedAt } ).ToList()
        });
    }

    public async Task<int> LikeSongAsync(Song song, Like like)
    {
        int newId = 1;
        var entity = await MockSongDatabase.GetSongById(song.Id);

        if( entity == null )
        {
            throw new SongNotFoundException(song.Id);
        }

        if( entity.Likes.Any( l => l.UserId == like.UserId ) )
        {
            throw new SongAlreadyLikedException(song.Id);
        }

        lock( _lock )
        {
            var lastLike = entity.Likes.OrderByDescending(l => l.Id).FirstOrDefault();

            if( lastLike != null )
            {
                newId = lastLike.Id + 1;
            }

            entity.Likes.Add( new music_streaming_infrastructure.Persistence.Like { Id = newId, SongId = song.Id, UserId = like.UserId, CreatedAt = DateTime.UtcNow });
        }

        return newId;
    }

    public async Task UnlikeSongAsync(Song song, int userId)
    {
        var song_entity = await MockSongDatabase.GetSongById(song.Id);

        if( song_entity == null )
        {
            throw new SongNotFoundException(song.Id);
        }

        var like_entity = song_entity.Likes.FirstOrDefault( like => like.UserId == userId );

        if( like_entity == null )
        {
            throw new SongIsntLikedException(song.Id);
        }

        song_entity.Likes.Remove(like_entity);
    }
}