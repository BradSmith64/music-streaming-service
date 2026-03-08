namespace music_streaming_application;

public class SongUploadedDomainEvent
{
    public required string BlobUri { get; set; }
    public required SongMetadataEvent Metadata { get; set; }
}

public class SongMetadataEvent
{
    public required string UserId { get; set; }
    public required DateTime UploadedAt { get; set; }
}