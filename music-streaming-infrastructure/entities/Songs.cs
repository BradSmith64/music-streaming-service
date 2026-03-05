namespace music_streaming_infrastructure.Persistence;

public class Song
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string AlbumTitle { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public required ICollection<Like> Likes { get; set; }
    public required string? FileName { get; set; }
}

public class Like
{
    public int Id { get; set; }
    public required int SongId { get; set; }
    public required int UserId { get; set; }
    public required DateTime CreatedAt { get; set; }
}