namespace music_streaming_application;

public class MetadataExtractionException : Exception
{
    public MetadataExtractionException(string blobUri, string reason, Exception? innerException = null) 
        : base($"Failed to extract metadata from {blobUri}. Reason: {reason}", innerException) { }
}

public class SongAlreadyRegisteredException : Exception
{
    public SongAlreadyRegisteredException(string title, string albumTitle) 
        : base($"The song '{title}' in album '{albumTitle}' is already registered in the system.") { }
}

public class StorageOperationException : Exception
{
    public StorageOperationException(string operation, string blobUri, Exception innerException) 
        : base($"Failed to perform {operation} on storage for URI: {blobUri}", innerException) { }
}
