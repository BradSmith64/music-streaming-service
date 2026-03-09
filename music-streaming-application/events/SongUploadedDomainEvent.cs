namespace music_streaming_application;

public class SongUploadedDomainEvent
{
    public required string BlobUri { get; set; }
    public required string UploaderId { get; set; }
}