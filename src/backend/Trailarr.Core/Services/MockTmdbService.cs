using Microsoft.Extensions.Logging;
using Trailarr.Core.Models;

namespace Trailarr.Core.Services;

public class MockTmdbService : ITmdbService
{
    private readonly ILogger<MockTmdbService> _logger;

    public MockTmdbService(ILogger<MockTmdbService> logger)
    {
        _logger = logger;
    }

    public Task<Movie?> GetMovieAsync(string title, int? year = null, CancellationToken token = default)
    {
        _logger.LogInformation("Using mock TMDb service for movie: {Title} ({Year})", title, year);
        var movie = new Movie
        {
            Title = title,
            FilePath = $"/movies/{title} ({year})/movie.mp4",
            Year = year,
            TmdbId = 0
        };
        return Task.FromResult<Movie?>(movie);
    }

    public Task<string?> GetTrailerUrlAsync(int tmdbId, CancellationToken token = default)
    {
        _logger.LogInformation("Using mock TMDb service for trailer: {TmdbId}", tmdbId);
        return Task.FromResult<string?>($"https://www.youtube.com/watch?v=mock-trailer-{tmdbId}");
    }
}
