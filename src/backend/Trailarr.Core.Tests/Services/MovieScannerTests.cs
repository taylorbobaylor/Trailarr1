using Microsoft.Extensions.Logging;
using Moq;
using Trailarr.Core.Models;
using Trailarr.Core.Services;
using Xunit;

namespace Trailarr.Core.Tests.Services;

public class MovieScannerTests
{
    private readonly Mock<ITmdbService> _tmdbService;
    private readonly Mock<ILogger<MovieScanner>> _logger;
    private readonly MovieScanner _scanner;

    public MovieScannerTests()
    {
        _tmdbService = new Mock<ITmdbService>();
        _logger = new Mock<ILogger<MovieScanner>>();
        _scanner = new MovieScanner(_tmdbService.Object, _logger.Object);
    }

    [Fact]
    public async Task ScanDirectoryAsync_WithValidMovie_ReturnsMovie()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), "MovieScannerTests");
        Directory.CreateDirectory(testDir);
        var moviePath = Path.Combine(testDir, "The Matrix (1999).mp4");
        File.WriteAllText(moviePath, "test");

        var expectedMovie = new Movie
        {
            Title = "The Matrix",
            FilePath = moviePath,
            Year = 1999,
            TmdbId = 603
        };

        _tmdbService.Setup(x => x.GetMovieAsync("The Matrix", 1999, default))
            .ReturnsAsync(expectedMovie);

        try
        {
            // Act
            var result = await _scanner.ScanDirectoryAsync(testDir);
            var movie = result.FirstOrDefault();

            // Assert
            Assert.NotNull(movie);
            Assert.Equal("The Matrix", movie.Title);
            Assert.Equal(1999, movie.Year);
            Assert.Equal(603, movie.TmdbId);
            Assert.Equal(moviePath, movie.FilePath);
        }
        finally
        {
            // Cleanup
            Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public async Task ScanDirectoryAsync_WithInvalidFileName_LogsWarning()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), "MovieScannerTests");
        Directory.CreateDirectory(testDir);
        var moviePath = Path.Combine(testDir, "invalid_movie_name.mp4");
        File.WriteAllText(moviePath, "test");

        try
        {
            // Act
            var result = await _scanner.ScanDirectoryAsync(testDir);

            // Assert
            Assert.Empty(result);
            _logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Could not find movie in TMDb")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
        finally
        {
            // Cleanup
            Directory.Delete(testDir, true);
        }
    }
}
