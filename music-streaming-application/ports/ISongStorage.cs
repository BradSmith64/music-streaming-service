namespace music_streaming_application;

public interface ISongStorage
{
    /// <summary>
    /// Generates a secure, public URI that the frontend can use to stream the audio file.
    /// </summary>
    public string? GeneratePublicStreamingUri(string? fileName);

    /// <summary>
    /// Opens a read-only stream to a file in the temporary landing zone.
    /// </summary>
    public Task<Stream> OpenLandingZoneStreamAsync(string blobUri);

    /// <summary>
    /// Moves the audio content into the permanent, production-ready music storage.
    /// </summary>
    public Task PersistToPermanentStorageAsync(string path, Stream content);

    /// <summary>
    /// Removes the temporary file from the landing zone after processing is complete.
    /// </summary>
    public Task PurgeFromLandingZoneAsync(string blobUri);
}