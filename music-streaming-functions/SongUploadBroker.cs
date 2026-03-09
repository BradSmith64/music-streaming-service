using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using music_streaming_application;

namespace music_streaming_functions;

public class SongUploadBroker
{
    private readonly ILogger<SongUploadBroker> _logger;

    public SongUploadBroker(ILogger<SongUploadBroker> logger)
    {
        _logger = logger;
    }

    [Function(nameof(SongUploadBroker))]
    [ServiceBusOutput("song-uploaded", Connection = "ServiceBus__ConnectionString")]
    public SongUploadedDomainEvent? Run([EventGridTrigger] EventGridEvent eventGridEvent)
    {
        _logger.LogInformation("Event Grid event received. Subject: {Subject}", eventGridEvent.Subject);

        // 1. Validate Event Type
        if (eventGridEvent.EventType != "Microsoft.Storage.BlobCreated")
        {
            _logger.LogWarning("Ignored event type: {EventType}", eventGridEvent.EventType);
            return null;
        }

        // 2. Extract Data
        var data = eventGridEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();
        if (data == null || string.IsNullOrEmpty(data.Url))
        {
            _logger.LogError("Event Grid event data is null or URL is missing.");
            return null;
        }
        
        var blobUri = data.Url;

        // 3. Validate File Extension (Anti-Corruption Layer)
        var extension = Path.GetExtension(blobUri).ToLowerInvariant();
        if (extension != ".mp3" && extension != ".wav")
        {
            _logger.LogWarning("Invalid file type uploaded: {Extension}. Ignoring.", extension);
            return null;
        }

        // 4. Extract UploaderId from Blob Path
        // Expected format: .../songs-landing-zone/{uploaderId}/{filename}
        var uriSegments = new Uri(blobUri).Segments;
        // Segments example: "/", "songs-landing-zone/", "user-123/", "song.mp3"
        if (uriSegments.Length < 3)
        {
            _logger.LogError("Blob URI format is invalid. Could not extract UploaderId. URI: {Uri}", blobUri);
            return null;
        }

        // The UploaderId is the segment before the filename
        var uploaderId = uriSegments[uriSegments.Length - 2].Trim('/');

        _logger.LogInformation("Valid song upload detected. Uploader: {UploaderId}, URI: {Uri}", uploaderId, blobUri);

        return new SongUploadedDomainEvent
        {
            BlobUri = blobUri,
            UploaderId = uploaderId
        };
    }

    // Helper class for deserializing Event Grid data
    private class StorageBlobCreatedEventData
    {
        public string Url { get; set; } = string.Empty;
    }
}