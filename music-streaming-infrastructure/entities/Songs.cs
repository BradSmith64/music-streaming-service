namespace music_streaming_infrastructure.Persistence;

public class Artist
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Album> Albums { get; set; } = new List<Album>();
}

public class Album
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int ArtistId { get; set; }
    public required Artist Artist { get; set; }
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}

public class Song
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int AlbumId { get; set; }
    public required Album Album { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public required ICollection<Like> Likes { get; set; } = new List<Like>();
    public required string FileName { get; set; }
}

public class Like
{
    public int Id { get; set; }
    public required int SongId { get; set; }
    public required int UserId { get; set; }
    public required DateTime CreatedAt { get; set; }
}