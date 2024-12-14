using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Trailarr.Core.Services;

public class TrailerDownloadBackgroundService(
    IMovieScanner movieScanner,
    ITrailerService trailerService,
    ILogger<TrailerDownloadBackgroundService> logger) : BackgroundService
{
    private readonly IMovieScanner _movieScanner = movieScanner;
    private readonly ITrailerService _trailerService = trailerService;
    private readonly ILogger<TrailerDownloadBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting trailer download scan...");

                // TODO: Get media library paths from configuration
                var paths = new[] { "/movies" };
                var movies = await _movieScanner.ScanDirectoriesAsync(paths, stoppingToken);

                await _trailerService.DownloadMissingTrailersAsync(movies, stoppingToken);

                // Wait for 1 hour before next scan
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Trailer download service stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in trailer download service");
        }
    }
}
