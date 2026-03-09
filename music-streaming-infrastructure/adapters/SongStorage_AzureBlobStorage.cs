using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using music_streaming_application;

namespace music_streaming_infrastructure;

public class SongStorage_AzureBlobStorage : ISongStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly string _landingZoneContainerName = "songs-landing-zone";
    private readonly string _accountName;
    private readonly string _accountKey;
    private readonly int _expiryMinutes;

    public SongStorage_AzureBlobStorage(SongStorage_AzureBlobStorageOptions options)
    {
        var connectionString = $"DefaultEndpointsProtocol=https;AccountName={options.AccountName};AccountKey={options.AccountKey};EndpointSuffix=core.windows.net";
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = options.ContainerName;
        _accountName = options.AccountName;
        _accountKey = options.AccountKey;
        _expiryMinutes = options.ExpiryMinutes ?? 60;
    }

    public string? GeneratePublicStreamingUri(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return null;

        var blobClient = _blobServiceClient.GetBlobContainerClient(_containerName).GetBlobClient(fileName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(_expiryMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_accountName, _accountKey)).ToString();

        return $"{blobClient.Uri}?{sasToken}";
    }

    public async Task<Stream> OpenLandingZoneStreamAsync(string blobUri)
    {
        var blobClient = new BlobClient(new Uri(blobUri), new StorageSharedKeyCredential(_accountName, _accountKey));
        
        // We buffer because TagLib# needs seekability
        var ms = new MemoryStream();
        await blobClient.DownloadToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public async Task PersistToPermanentStorageAsync(string fileName, Stream content)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        
        await blobClient.UploadAsync(content, overwrite: true);
    }

    public async Task PurgeFromLandingZoneAsync(string blobUri)
    {
        var blobClient = new BlobClient(new Uri(blobUri), new StorageSharedKeyCredential(_accountName, _accountKey));
        await blobClient.DeleteIfExistsAsync();
    }
}

public class SongStorage_AzureBlobStorageOptions
{
    public required string AccountName { get; set; }
    public required string AccountKey { get; set; }
    public required string ContainerName { get; set; }
    public int? ExpiryMinutes { get; set; }
}