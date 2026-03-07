using music_streaming_application;

namespace music_streaming_infrastructure;

public class SongStorage_LocalHttpServer : ISongStorage
{
    private readonly string _baseUrl;

    public SongStorage_LocalHttpServer(string baseUrl = "http://localhost:8080/")
    {
        _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
    }

    public string? GetFileUri(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        return _baseUrl + fileName;
    }

    public Task DeleteFileAsync(string blobUri)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> OpenReadStreamAsync(string blobUri)
    {
        throw new NotImplementedException();
    }

    public Task UploadFileAsync(string path, Stream content)
    {
        throw new NotImplementedException();
    }
}
