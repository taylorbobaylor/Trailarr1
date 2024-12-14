using Microsoft.Extensions.Logging;
using Moq;
using Trailarr.Core.Models;
using Trailarr.Core.Services;
using Xunit;
using YoutubeExplode.Converter;

namespace Trailarr.Core.Tests.Services;

public class TrailerServiceTests
{
    private readonly Mock<ITmdbService> _tmdbService;
    private readonly Mock<ILogger<TrailerService>> _logger;
    private readonly Mock<IYoutubeClient> _youtube;
    private readonly TrailerService _service;

    public TrailerServiceTests()
    {
        _tmdbService = new Mock<ITmdbService>();
        _logger = new Mock<ILogger<TrailerService>>();
        _youtube = new Mock<IYoutubeClient>();
        _service = new TrailerService(_tmdbService.Object, _logger.Object, _youtube.Object);
    }

    [Fact]
    public async Task DownloadTrailerAsync_WithValidMovie_SavesTrailerWithCorrectSuffix()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), "TrailerServiceTests");
        Directory.CreateDirectory(testDir);
        var moviePath = Path.Combine(testDir, "The Matrix (1999).mp4");
        File.WriteAllText(moviePath, "test");

        var movie = new Movie
        {
            Title = "The Matrix",
            FilePath = moviePath,
            Year = 1999,
            TmdbId = 603
        };

        var trailerUrl = "https://www.youtube.com/watch?v=m8e-FF8MsqU";
        _tmdbService.Setup(x => x.GetTrailerUrlAsync(603, default))
            .ReturnsAsync(trailerUrl);

        _youtube.Setup(x => x.DownloadAsync(
            trailerUrl,
            It.IsAny<string>(),
            It.IsAny<Action<ConversionRequestBuilder>>(),
            default))
            .Returns(Task.CompletedTask);

        try
        {
            // Act
            var result = await _service.DownloadTrailerAsync(movie);

            // Assert
            Assert.True(result);
            var expectedTrailerPath = Path.ChangeExtension(moviePath, null) + "-trailer.mp4";
            Assert.Equal(expectedTrailerPath, movie.TrailerPath);
            _logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully downloaded trailer")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            _youtube.Verify(x => x.DownloadAsync(
                trailerUrl,
                It.IsAny<string>(),
                It.IsAny<Action<ConversionRequestBuilder>>(),
                default), Times.Once);
        }
        finally
        {
            // Cleanup
            Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public async Task DownloadTrailerAsync_WithNoTrailerAvailable_LogsWarning()
    {
        // Arrange
        var movie = new Movie
        {
            Title = "Non-existent Movie",
            FilePath = "/path/to/movie.mp4",
            Year = 2024,
            TmdbId = 999999
        };

        _tmdbService.Setup(x => x.GetTrailerUrlAsync(999999, default))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _service.DownloadTrailerAsync(movie);

        // Assert
        Assert.False(result);
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No trailer found for movie")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
