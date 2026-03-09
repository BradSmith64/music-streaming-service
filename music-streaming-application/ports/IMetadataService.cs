namespace music_streaming_application;

public interface IMetadataService
{
    public Task<ID3Metadata> ExtractMetadataAsync(Stream audioStream, string fileName);
}


public class ID3Metadata
{
    public required string Title { get; set; }
    public required string AlbumTitle { get; set; }
    public string? Artist { get; set; }
    public DateTime? ReleaseDate { get; set; }
}