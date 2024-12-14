using Trailarr.Core.Models;

namespace Trailarr.Core.Services;

public interface ITrailerService
{
    Task<bool> DownloadTrailerAsync(Movie movie, CancellationToken token = default);
    Task<bool> HasTrailerAsync(Movie movie);
    Task DownloadMissingTrailersAsync(IEnumerable<Movie> movies, CancellationToken token = default);
}
