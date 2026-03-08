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
        var blobUri = "https://storage.blob.core.windows.net/landing/song.mp3";
        var uploaderId = "user-123";
        var command = new RegisterSongCommand { BlobUri = blobUri, UploaderId = uploaderId };
        
        var metadata = new ID3Metadata
        {
            Title = "Bohemian Rhapsody",
            AlbumTitle = "A Night at the Opera",
            Artist = "Queen"
        };

        _mockStorage.SetupSequence(s => s.OpenReadStreamAsync(blobUri))
            .ReturnsAsync(new MemoryStream())
            .ReturnsAsync(new MemoryStream());

        _mockMetadataService.Setup(m => m.ExtractMetadataAsync(It.IsAny<Stream>())).ReturnsAsync(metadata);
        
        _mockRepository.Setup(r => r.GetSongByTitleAndAlbumAsync(metadata.Title, metadata.AlbumTitle))
            .ReturnsAsync((Song?)null);

        // Artist and Album don't exist
        _mockRepository.Setup(r => r.GetArtistByNameAsync(metadata.Artist)).ReturnsAsync((Artist?)null);
        _mockRepository.Setup(r => r.AddArtistAsync(It.IsAny<Artist>())).ReturnsAsync(10); // Assigned ID
        
        _mockRepository.Setup(r => r.GetAlbumByTitleAndArtistAsync(metadata.AlbumTitle, 10)).ReturnsAsync((Album?)null);
        _mockRepository.Setup(r => r.AddAlbumAsync(It.IsAny<Album>())).ReturnsAsync(20); // Assigned ID

        // Act
        await _useCase.ExecuteAsync(command);

        // Assert
        _mockRepository.Verify(r => r.AddArtistAsync(It.Is<Artist>(a => a.Name == "Queen")), Times.Once);
        _mockRepository.Verify(r => r.AddAlbumAsync(It.Is<Album>(a => a.Title == "A Night at the Opera" && a.Artist.Id == 10)), Times.Once);
        _mockRepository.Verify(r => r.AddSongAsync(It.Is<Song>(s => s.Title == "Bohemian Rhapsody" && s.Album.Id == 20)), Times.Once);
        _mockStorage.Verify(s => s.DeleteFileAsync(blobUri), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Skip_Registration_When_Song_Already_Exists()
    {
        // Arrange
        var blobUri = "https://storage.blob.core.windows.net/landing/song.mp3";
        var command = new RegisterSongCommand { BlobUri = blobUri, UploaderId = "user-123" };
        
        var metadata = new ID3Metadata
        {
            Title = "Existing Song",
            AlbumTitle = "Existing Album"
        };

        _mockStorage.Setup(s => s.OpenReadStreamAsync(blobUri)).ReturnsAsync(new MemoryStream());
        _mockMetadataService.Setup(m => m.ExtractMetadataAsync(It.IsAny<Stream>())).ReturnsAsync(metadata);
        
        _mockRepository.Setup(r => r.GetSongByTitleAndAlbumAsync(metadata.Title, metadata.AlbumTitle))
            .ReturnsAsync(new Song { 
                Id = 1, 
                Title = "Existing Song", 
                Album = new Album { Id = 2, Title = "Existing Album", Artist = new Artist { Id = 3, Name = "Artist" } },
                FileName = "song.mp3", 
                Likes = new List<Like>() 
            });

        // Act
        await _useCase.ExecuteAsync(command);

        // Assert
        _mockRepository.Verify(r => r.AddSongAsync(It.IsAny<Song>()), Times.Never);
        _mockStorage.Verify(s => s.DeleteFileAsync(blobUri), Times.Never);
    }
}