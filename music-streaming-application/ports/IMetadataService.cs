namespace music_streaming_application;

public interface IMetadataService
{
    Task<ID3Metadata> ExtractMetadataAsync(Stream audioStream);
}

public class ID3Metadata
{
    public required string Title { get; set; }
    public required string AlbumTitle { get; set; }
    public string? Artist { get; set; }
    public DateTime? ReleaseDate { get; set; }
}