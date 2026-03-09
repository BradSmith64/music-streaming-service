using Moq;
using music_streaming_application;
using music_streaming_domain.Songs;
using Xunit;

namespace music_streaming_application.tests;

public class RegisterSongUseCaseTests
{
    private readonly Mock<ISongRepository> _mockRepository;
    private readonly Mock<ISongStorage> _mockStorage;
    private readonly Mock<IMetadataService> _mockMetadataService;
    private readonly RegisterSongUseCase _useCase;

    public RegisterSongUseCaseTests()
    {
        _mockRepository = new Mock<ISongRepository>();
        _mockStorage = new Mock<ISongStorage>();
        _mockMetadataService = new Mock<IMetadataService>();
        _useCase = new RegisterSongUseCase(_mockRepository.Object, _mockStorage.Object, _mockMetadataService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Register_Song_With_New_Artist_And_Album()
    {
        // Arrange
        var blobUri = "https://storage.blob.core.windows.net/landing/song.wav";
        var uploaderId = "user-123";
        var command = new RegisterSongCommand { BlobUri = blobUri, UploaderId = uploaderId };
        
        var metadata = new ID3Metadata
        {
            Title = "Bohemian Rhapsody",
            AlbumTitle = "A Night at the Opera",
            Artist = "Queen"
        };

        var expectedName = "user-123-a-night-at-the-opera-bohemian-rhapsody.wav";

        _mockStorage.SetupSequence(s => s.OpenLandingZoneStreamAsync(blobUri))
            .ReturnsAsync(new MemoryStream())
            .ReturnsAsync(new MemoryStream());

        _mockMetadataService.Setup(m => m.ExtractMetadataAsync(It.IsAny<Stream>(), blobUri)).ReturnsAsync(metadata);
        
        _mockRepository.Setup(r => r.GetArtistByNameAsync(metadata.Artist)).ReturnsAsync((Artist?)null);
        _mockRepository.Setup(r => r.AddArtistAsync(It.IsAny<Artist>())).ReturnsAsync(10);
        
        _mockRepository.Setup(r => r.GetAlbumByTitleAndArtistAsync(metadata.AlbumTitle, 10)).ReturnsAsync((Album?)null);
        _mockRepository.Setup(r => r.AddAlbumAsync(It.IsAny<Album>())).ReturnsAsync(20);

        _mockRepository.Setup(r => r.GetSongByTitleAlbumAndArtistAsync(metadata.Title, metadata.AlbumTitle, 10))
            .ReturnsAsync((Song?)null);

        // Act
        await _useCase.ExecuteAsync(command);

        // Assert
        _mockStorage.Verify(s => s.PersistToPermanentStorageAsync(expectedName, It.IsAny<Stream>()), Times.Once);
        _mockRepository.Verify(r => r.AddSongAsync(It.Is<Song>(s => s.Title == "Bohemian Rhapsody")), Times.Once);
        _mockStorage.Verify(s => s.PurgeFromLandingZoneAsync(blobUri), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Song_Already_Exists()
    {
        // Arrange
        var blobUri = "https://storage.blob.core.windows.net/landing/song.wav";
        var command = new RegisterSongCommand { BlobUri = blobUri, UploaderId = "user-123" };

        var metadata = new ID3Metadata
        {
            Title = "Existing Song",
            AlbumTitle = "Existing Album",
            Artist = "Existing Artist"
        };

        _mockStorage.Setup(s => s.OpenLandingZoneStreamAsync(blobUri)).ReturnsAsync(new MemoryStream());
        _mockMetadataService.Setup(m => m.ExtractMetadataAsync(It.IsAny<Stream>(), blobUri)).ReturnsAsync(metadata);

        _mockRepository.Setup(r => r.GetArtistByNameAsync("Existing Artist")).ReturnsAsync(new Artist { Id = 10, Name = "Existing Artist" });
        _mockRepository.Setup(r => r.GetAlbumByTitleAndArtistAsync("Existing Album", 10)).ReturnsAsync(new Album { Id = 20, Title = "Existing Album", Artist = new Artist { Id = 10, Name = "Artist" } });

        _mockRepository.Setup(r => r.GetSongByTitleAlbumAndArtistAsync(metadata.Title, metadata.AlbumTitle, 10))
            .ReturnsAsync(new Song { 
                Id = 1, 
                Title = "Existing Song", 
                Album = new Album { Id = 20, Title = "Existing Album", Artist = new Artist { Id = 10, Name = "Artist" } },
                FileName = "song.wav", 
                Likes = new List<Like>() 
            });

        // Act & Assert
        await Assert.ThrowsAsync<SongAlreadyRegisteredException>(() => _useCase.ExecuteAsync(command));
        
        _mockRepository.Verify(r => r.AddSongAsync(It.IsAny<Song>()), Times.Never);
        _mockStorage.Verify(s => s.PurgeFromLandingZoneAsync(blobUri), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_MetadataExtractionException_When_Metadata_Extraction_Fails()
    {
        // Arrange
        var blobUri = "https://storage.blob.core.windows.net/landing/bad.wav";
        var command = new RegisterSongCommand { BlobUri = blobUri, UploaderId = "user-123" };

        _mockStorage.Setup(s => s.OpenLandingZoneStreamAsync(blobUri)).ThrowsAsync(new Exception("Network error"));

        // Act & Assert
        await Assert.ThrowsAsync<MetadataExtractionException>(() => _useCase.ExecuteAsync(command));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_StorageOperationException_When_Persistence_Fails()
    {
        // Arrange
        var blobUri = "https://storage.blob.core.windows.net/landing/song.wav";
        var command = new RegisterSongCommand { BlobUri = blobUri, UploaderId = "user-123" };
        
        var metadata = new ID3Metadata { Title = "Title", AlbumTitle = "Album", Artist = "Artist" };

        _mockStorage.SetupSequence(s => s.OpenLandingZoneStreamAsync(blobUri))
            .ReturnsAsync(new MemoryStream()) // 1st call for metadata
            .ReturnsAsync(new MemoryStream()); // 2nd call for persistence

        _mockMetadataService.Setup(m => m.ExtractMetadataAsync(It.IsAny<Stream>(), blobUri)).ReturnsAsync(metadata);
        _mockRepository.Setup(r => r.GetArtistByNameAsync(It.IsAny<string>())).ReturnsAsync(new Artist { Id = 1, Name = "Artist" });
        _mockRepository.Setup(r => r.GetAlbumByTitleAndArtistAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new Album { Id = 1, Title = "Album", Artist = new Artist { Id = 1, Name = "Artist" } });
        _mockRepository.Setup(r => r.GetSongByTitleAlbumAndArtistAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((Song?)null);

        _mockStorage.Setup(s => s.PersistToPermanentStorageAsync(It.IsAny<string>(), It.IsAny<Stream>()))
            .ThrowsAsync(new Exception("Disk full"));

        // Act & Assert
        await Assert.ThrowsAsync<StorageOperationException>(() => _useCase.ExecuteAsync(command));
    }
}