using music_streaming_application;
using TagLib;

namespace music_streaming_infrastructure;

public class MetadataService_TagLib : IMetadataService
{
    public async Task<ID3Metadata> ExtractMetadataAsync(Stream audioStream, string fileName)
    {
        // TagLib# needs a seekable stream or a specific abstraction for non-seekable streams.
        // For simplicity in this PoC, we assume the stream is seekable or wrap it.
        
        var tfile = TagLib.File.Create(new StreamFileAbstraction(fileName, audioStream, audioStream));
        
        var metadata = new ID3Metadata
        {
            Title = tfile.Tag.Title ?? "Unknown Title",
            AlbumTitle = tfile.Tag.Album ?? "Unknown Album",
            Artist = tfile.Tag.FirstPerformer ?? "Unknown Artist",
            ReleaseDate = tfile.Tag.Year != 0 ? new DateTime((int)tfile.Tag.Year, 1, 1) : null
        };

        return await Task.FromResult(metadata);
    }
}

// Helper abstraction for TagLib# to read from a .NET Stream
public class StreamFileAbstraction : TagLib.File.IFileAbstraction
{
    public StreamFileAbstraction(string name, Stream readStream, Stream writeStream)
    {
        Name = name;
        ReadStream = readStream;
        WriteStream = writeStream;
    }

    public string Name { get; }
    public Stream ReadStream { get; }
    public Stream WriteStream { get; }

    public void CloseStream(Stream stream)
    {
        // We don't want TagLib to close our streams, the caller handles that.
    }
}