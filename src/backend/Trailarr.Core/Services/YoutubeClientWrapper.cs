using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Search;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Trailarr.Core.Services;

public class YoutubeClientWrapper : IYoutubeClient
{
    private readonly YoutubeClient _client;
    private readonly ILogger<YoutubeClientWrapper> _logger;

    public YoutubeClientWrapper(ILogger<YoutubeClientWrapper> logger)
    {
        _client = new YoutubeClient();
        _logger = logger;
    }

    public async Task<string?> GetBestTrailerUrlAsync(string movieTitle, int? year = null, CancellationToken token = default)
    {
        try
        {
            var searchQuery = $"{movieTitle} {year} official trailer";
            _logger.LogInformation("Searching for trailer: {Query}", searchQuery);

            // Search for videos and get the first result that matches our criteria
            var searchResults = _client.Search.GetVideosAsync(searchQuery, token);
            var bestMatch = await searchResults
                .Where(v => v.Title.Contains("trailer", StringComparison.OrdinalIgnoreCase) &&
                           v.Duration.GetValueOrDefault().TotalMinutes < 5)
                .FirstOrDefaultAsync(token);

            return bestMatch?.Url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trailer URL for movie: {Title} ({Year})", movieTitle, year);
            return null;
        }
    }

    public async Task DownloadVideoAsync(string videoUrl, string outputPath, CancellationToken token = default)
    {
        try
        {
            _logger.LogInformation("Downloading video from {Url} to {Path}", videoUrl, outputPath);

            var video = await _client.Videos.GetAsync(videoUrl, token);
            var streamManifest = await _client.Videos.Streams.GetManifestAsync(video.Id, token);

            // Get best quality muxed stream (audio + video)
            var streamInfo = streamManifest.GetMuxedStreams()
                .OrderByDescending(s => s.VideoQuality)
                .FirstOrDefault();

            if (streamInfo == null)
            {
                throw new InvalidOperationException("No suitable video stream found");
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create progress handler
            var progress = new Progress<double>(p =>
                _logger.LogInformation("Download progress: {Progress:P2}", p));

            // Download the stream
            await _client.Videos.Streams.DownloadAsync(streamInfo, outputPath, progress, token);
            _logger.LogInformation("Successfully downloaded video to {Path}", outputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading video from {Url} to {Path}", videoUrl, outputPath);
            throw;
        }
    }
}
