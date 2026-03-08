namespace music_streaming_domain.Songs;

public class Artist
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}

public class Album
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required Artist Artist { get; set; }
}

public class Song
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required Album Album { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public required List<Like> Likes { get; set; }
    public required string FileName { get; set; }

    public Like Like(int userId)
    {
        if( Likes.Any( like => like.UserId == userId ))
        {
            throw new SongAlreadyLikedException(this.Id);
        }

        var newLike =  new Like { SongId = this.Id, UserId = userId, CreatedAt = DateTime.UtcNow };

        Likes.Add(newLike);

        return newLike;
    }

    public void Unlike(int userId)
    {
        var like = this.Likes.FirstOrDefault(l => l.UserId == userId);

        if( like == null )
        {
            throw new SongIsntLikedException(this.Id);
        }

        Likes.Remove(like);
    }
}

public class Like
{
    public int? Id { get; set; }
    public required int UserId { get; set; }
    public required int SongId { get; set; }
    public required DateTime CreatedAt { get; set; }
}

// Exceptions remain the same...
public class SongNotFoundException : Exception
{
    public SongNotFoundException(int songId) : base($"The requested song (with ID {songId}) was not found") { }
}

public class SongAlreadyLikedException : Exception
{
    public SongAlreadyLikedException(int songId) : base($"The requested song (with ID {songId}) has already been liked by the user") { }
}

public class SongIsntLikedException : Exception
{
    public SongIsntLikedException(int songId) : base($"The requested song (with ID {songId}) is not currently liked by the user") { }
}