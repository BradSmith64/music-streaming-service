using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using music_streaming_application;

namespace music_streaming_infrastructure;

public class SongStorage_AzureBlobStorage : ISongStorage
{
    private readonly StorageSharedKeyCredential _credentials;
    private readonly string _accountName;
    private readonly string _accountKey;
    private readonly string _containerName;
    private readonly int _expiryMinutes;


    public SongStorage_AzureBlobStorage(SongStorage_AzureBlobStorageOptions options)
    {
        _accountName = options.AccountName;
        _accountKey = options.AccountKey;
        _containerName = options.ContainerName;
        _expiryMinutes = options.ExpiryMinutes ?? 60;

        _credentials = new StorageSharedKeyCredential(options.AccountName, options.AccountKey);
    }

    public string? GetFileUri(string? fileName)
    {
        if( fileName == null)
        {
            return null;
        }

        // Create blob client
        var blobClient = new BlobClient(
            new Uri($"https://{_accountName}.blob.core.windows.net/{_containerName}/{fileName}"),
            _credentials);

        // Build SAS
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = fileName,
            Resource = "b", // "b" = blob, "c" = container
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(_expiryMinutes)
        };

        // Permissions
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Generate full SAS URI
        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

        Console.WriteLine($"Generated SAS token {sasUri.ToString()}");

        return sasUri.ToString();
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

public class SongStorage_AzureBlobStorageOptions
{
    public required string AccountName { get; set; }
    public required string AccountKey { get; set; }
    public required string ContainerName { get; set; }
    public int? ExpiryMinutes { get; set; }
}