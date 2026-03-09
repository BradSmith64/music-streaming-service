using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using music_streaming_application;

namespace music_streaming_functions;

public class SongProcessor
{
    private readonly ILogger<SongProcessor> _logger;
    private readonly RegisterSongUseCase _useCase;

    public SongProcessor(ILogger<SongProcessor> logger, RegisterSongUseCase useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [Function(nameof(SongProcessor))]
    public async Task Run([ServiceBusTrigger("song-uploaded", Connection = "ServiceBus__ConnectionString")] SongUploadedDomainEvent domainEvent)
    {
        _logger.LogInformation("Processing song upload for Uploader: {UploaderId}, URI: {BlobUri}", 
            domainEvent.UploaderId, domainEvent.BlobUri);

        var command = new RegisterSongCommand
        {
            BlobUri = domainEvent.BlobUri,
            UploaderId = domainEvent.UploaderId
        };

        try
        {
            await _useCase.ExecuteAsync(command);
            _logger.LogInformation("Successfully registered song: {BlobUri}", domainEvent.BlobUri);
        }
        catch (MetadataExtractionException ex)
        {
            _logger.LogError(ex, "DATA_ERROR: Metadata extraction failed for {BlobUri}. Moving to DLQ after retries.", domainEvent.BlobUri);
            throw; // Rethrow to allow DLQ
        }
        catch (SongAlreadyRegisteredException ex)
        {
            _logger.LogWarning("IDEMPOTENCY: {Message} Skipping processing.", ex.Message);
            // We DON'T throw here because we want to "Complete" the message and remove it from the queue.
            // Processing is technically successful (it's already done).
        }
        catch (StorageOperationException ex)
        {
            _logger.LogError(ex, "STORAGE_ERROR: Operation failed for {BlobUri}. Will retry.", domainEvent.BlobUri);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UNEXPECTED_ERROR: Failed to process song upload: {BlobUri}", domainEvent.BlobUri);
            throw; 
        }
    }
}