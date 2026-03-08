using System.ComponentModel.DataAnnotations;

namespace music_streaming_domain;

public class SongMetadata
{
    public required int SongId { get; set; }
    public required string Title { get; set; }
    public required string AlbumTitle { get; set; }
    public required string ArtistName { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public required int LikeCount { get; set; }
    public required bool LikedByUser { get; set; }
    public required string? FileName { get; set; }
    public required string? Url { get; set; }
}