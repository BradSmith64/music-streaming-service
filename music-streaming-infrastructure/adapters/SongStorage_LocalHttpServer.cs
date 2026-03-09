using music_streaming_application;

namespace music_streaming_infrastructure;

public class SongStorage_LocalHttpServer : ISongStorage
{
    private readonly string _baseUrl;
    private readonly string _localMusicPath;
    private readonly string _localLandingZonePath;
    private readonly HttpClient _httpClient;

    public SongStorage_LocalHttpServer(SongStorage_LocalHttpServerOptions options)
    {
        _baseUrl = options.BaseUrl.EndsWith("/") ? options.BaseUrl : options.BaseUrl + "/";
        _httpClient = new HttpClient();
        
        // Paths are now injected via DI
        _localMusicPath = options.LocalMusicPath;
        _localLandingZonePath = options.LocalLandingZonePath;

        if (!Directory.Exists(_localMusicPath)) Directory.CreateDirectory(_localMusicPath);
        if (!Directory.Exists(_localLandingZonePath)) Directory.CreateDirectory(_localLandingZonePath);
    }

    public string? GeneratePublicStreamingUri(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        return _baseUrl + "music/" + fileName;
    }

    public async Task<Stream> OpenLandingZoneStreamAsync(string blobUri)
    {
        // Simulation: Use HttpClient to pull from the locally running http-server
        var response = await _httpClient.GetAsync(blobUri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        
        // We buffer into a MemoryStream because TagLib# needs a seekable stream
        var ms = new MemoryStream();
        await response.Content.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public async Task PersistToPermanentStorageAsync(string fileName, Stream content)
    {
        var filePath = Path.Combine(_localMusicPath, fileName);
        using var fileStream = File.Create(filePath);
        await content.CopyToAsync(fileStream);
    }

    public async Task PurgeFromLandingZoneAsync(string blobUri)
    {
        // 1. Get the local path of the URI
        var localPath = new Uri(blobUri).LocalPath; // e.g. "/landing-zone/user-123/song.mp3"

        // 2. Identify the part of the path that is relative to the landing-zone container
        // Azure structure: /landing-zone/{uploaderId}/{fileName}
        var segments = localPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        if (segments.Length >= 3 && segments[0].Equals("landing-zone", StringComparison.OrdinalIgnoreCase))
        {
            // Join everything after "landing-zone" back into a relative path
            var relativePath = Path.Combine(segments.Skip(1).ToArray());
            var filePath = Path.Combine(_localLandingZonePath, relativePath);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        else
        {
            // Fallback for simple/legacy local uploads (root of landing zone)
            var fileName = Path.GetFileName(localPath);
            var filePath = Path.Combine(_localLandingZonePath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
        await Task.CompletedTask;
    }
}

public class SongStorage_LocalHttpServerOptions
{
    public const string SectionName = "LocalHttpServer";
    public required string BaseUrl { get; set; }
    public required string LocalMusicPath { get; set; }
    public required string LocalLandingZonePath { get; set; }
}