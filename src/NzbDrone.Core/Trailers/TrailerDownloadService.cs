using System;
using System.IO;
using System.Threading.Tasks;
using NLog;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Converter;

namespace NzbDrone.Core.Trailers
{
    public class TrailerDownloadService : ITrailerDownloadService
    {
        private readonly ILogger _logger;
        private readonly YoutubeClient _youtube;

        public TrailerDownloadService(ILogger logger)
        {
            _logger = logger;
            _youtube = new YoutubeClient();
        }

        public async Task<bool> DownloadTrailerAsync(string movieTitle, string trailerId, string outputPath)
        {
            try
            {
                _logger.Debug($"Starting trailer download for movie: {movieTitle}");

                if (string.IsNullOrWhiteSpace(trailerId))
                {
                    _logger.Warning($"No trailer ID provided for movie: {movieTitle}");
                    return false;
                }

                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Download the video
                await _youtube.Videos.DownloadAsync(trailerId, outputPath);

                _logger.Debug($"Successfully downloaded trailer for movie: {movieTitle}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to download trailer for movie: {movieTitle}");
                return false;
            }
        }

        public async Task<string> GetBestTrailerUrlAsync(string movieTitle)
        {
            try
            {
                var searchResults = await _youtube.Search.GetVideosAsync($"{movieTitle} official trailer");
                var firstResult = await searchResults.FirstOrDefaultAsync();
                return firstResult?.Url;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to find trailer for movie: {movieTitle}");
                return null;
            }
        }
    }
}
