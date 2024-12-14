namespace NzbDrone.Core.Movies;

public interface ITmdbService
{
    Task<string?> GetTrailerUrlAsync(int tmdbId, CancellationToken token = default);
    Task<Movie?> GetMovieAsync(int tmdbId, CancellationToken token = default);
}
