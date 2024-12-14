using Trailarr.Core.Models;

namespace Trailarr.Core.Services;

public interface ITmdbService
{
    Task<string?> GetTrailerUrlAsync(int tmdbId, CancellationToken token = default);
    Task<Movie?> GetMovieAsync(string title, int? year = null, CancellationToken token = default);
}
