namespace Trailarr.Core.Services;

public interface IYoutubeClient
{
    Task<string?> GetBestTrailerUrlAsync(string movieTitle, int? year = null, CancellationToken token = default);
    Task DownloadVideoAsync(string videoUrl, string outputPath, CancellationToken token = default);
}
