using System.IO;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Trailarr.Core.Services;

public class TrailerService(ITmdbService tmdbService, ILogger<TrailerService> logger) : ITrailerService
{
    private readonly ITmdbService _tmdbService = tmdbService;
    private readonly ILogger<TrailerService> _logger = logger;
    private readonly YoutubeClient _youtube = new();

    public async Task<bool> DownloadTrailerAsync(Models.Movie movie, CancellationToken token = default)
    {
        try
        {
            if (movie.TmdbId == null)
            {
                _logger.LogWarning("Cannot download trailer for movie without TMDb ID: {Title}", movie.Title);
                return false;
            }

            var trailerUrl = await _tmdbService.GetTrailerUrlAsync(movie.TmdbId.Value, token);
            if (string.IsNullOrEmpty(trailerUrl))
            {
                _logger.LogWarning("No trailer found for movie: {Title}", movie.Title);
                return false;
            }

            var trailerPath = Path.ChangeExtension(movie.FilePath, null) + "-trailer.mp4";
            if (File.Exists(trailerPath))
            {
                _logger.LogInformation("Trailer already exists for movie: {Title}", movie.Title);
                movie.TrailerPath = trailerPath;
                return true;
            }

            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(trailerUrl, token);
            var streamInfo = streamManifest.GetMuxedStreams()
                .OrderByDescending(s => s.VideoQuality)
                .FirstOrDefault();

            if (streamInfo == null)
            {
                _logger.LogWarning("No suitable video stream found for movie trailer: {Title}", movie.Title);
                return false;
            }

            await _youtube.Videos.Streams.DownloadAsync(streamInfo, trailerPath, cancellationToken: token);
            movie.TrailerPath = trailerPath;
            _logger.LogInformation("Successfully downloaded trailer for movie: {Title}", movie.Title);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading trailer for movie: {Title}", movie.Title);
            return false;
        }
    }

    public Task<bool> HasTrailerAsync(Models.Movie movie)
    {
        var trailerPath = Path.ChangeExtension(movie.FilePath, null) + "-trailer.mp4";
        return Task.FromResult(File.Exists(trailerPath));
    }

    public async Task DownloadMissingTrailersAsync(IEnumerable<Models.Movie> movies, CancellationToken token = default)
    {
        foreach (var movie in movies)
        {
            if (await HasTrailerAsync(movie))
            {
                continue;
            }

            await DownloadTrailerAsync(movie, token);
        }
    }
}
