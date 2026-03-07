namespace music_streaming_application;

public interface ISongStorage
{
    public string? GetFileUri(string? fileName);
    public Task<Stream> OpenReadStreamAsync(string blobUri);
    public Task UploadFileAsync(string path, Stream content);
    public Task DeleteFileAsync(string blobUri);
}