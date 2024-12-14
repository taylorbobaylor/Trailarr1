using System.IO;
using Microsoft.Extensions.Logging;
using Trailarr.Core.Models;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace Trailarr.Core.Services;

public interface IYoutubeClient
{
    Task DownloadAsync(string url, string outputPath, Action<ConversionRequestBuilder> configure, CancellationToken cancellationToken = default);
}

public class YoutubeClientWrapper : IYoutubeClient
{
    private readonly YoutubeClient _client = new();

    public async Task DownloadAsync(string url, string outputPath, Action<ConversionRequestBuilder> configure, CancellationToken cancellationToken = default)
    {
        await _client.Videos.DownloadAsync(url, outputPath, configure, cancellationToken);
    }
}

public class TrailerService : ITrailerService
{
    private readonly ITmdbService _tmdbService;
    private readonly ILogger<TrailerService> _logger;
    private readonly IYoutubeClient _youtube;

    public TrailerService(ITmdbService tmdbService, ILogger<TrailerService> logger, IYoutubeClient? youtube = null)
    {
        _tmdbService = tmdbService;
        _logger = logger;
        _youtube = youtube ?? new YoutubeClientWrapper();
    }

    public async Task<bool> DownloadTrailerAsync(Movie movie, CancellationToken token = default)
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

            await _youtube.DownloadAsync(trailerUrl, trailerPath,
                builder => builder
                    .SetPreset(ConversionPreset.UltraFast)
                    .SetContainer("mp4")
                    .SetFFmpegPath("ffmpeg"),
                token);

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

    public Task<bool> HasTrailerAsync(Movie movie)
    {
        var trailerPath = Path.ChangeExtension(movie.FilePath, null) + "-trailer.mp4";
        return Task.FromResult(File.Exists(trailerPath));
    }

    public async Task DownloadMissingTrailersAsync(IEnumerable<Movie> movies, CancellationToken token = default)
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
