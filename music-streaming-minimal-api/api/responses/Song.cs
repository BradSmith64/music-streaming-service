namespace music_streaming_minimal_api;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class GetSongsResponseDTO
{
    [JsonPropertyName("items"), Required]
    public required List<SongDTO> Items { get; set; }
}

public class SongDTO
{
    [JsonPropertyName("id"), Required]
    public required int SongId { get; set; }
    
    [JsonPropertyName("title"), Required]
    public required string Title { get; set; }
    
    [JsonPropertyName("albumTitle"), Required]
    public required string AlbumTitle { get; set; }

    [JsonPropertyName("releaseDate")]
    public DateTime? ReleaseDate { get; set; }    

    [JsonPropertyName("likeCount"), Required]
    public required int LikeCount { get; set; }

    [JsonPropertyName("liked"), Required]
    public required bool LikedByUser { get; set; }

    [JsonPropertyName("url")]
    public required string? Url { get; set; }
}

public class LikeSongDTO
{
    [JsonPropertyName("id"), Required]
    public required int Id { get; set; }
}