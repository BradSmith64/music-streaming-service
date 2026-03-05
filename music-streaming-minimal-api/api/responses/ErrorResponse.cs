namespace music_streaming_minimal_api;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class ErrorResponseDTO
{
    [JsonPropertyName("error"), Required]
    public required string ErrorMessage { get; set; }

    [JsonPropertyName("code")]
    public string? ErrorCode { get; set; }
}
