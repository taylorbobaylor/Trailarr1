using TMDbLib.Client;
using Serilog;

namespace NzbDrone.Core.Movies;

public class TmdbService : ITmdbService
{
    private readonly TMDbClient _client;

    public TmdbService(IConfigFileProvider configFileProvider)
    {
        var apiKey = configFileProvider.GetValue("TMDbApiKey", string.Empty);
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("TMDb API key is not configured");
        }
        _client = new TMDbClient(apiKey);
    }

    public async Task<string?> GetTrailerUrlAsync(int tmdbId, CancellationToken token = default)
    {
        try
        {
            var movie = await _client.GetMovieAsync(tmdbId, MovieMethods.Videos, cancellationToken: token);
            var trailer = movie.Videos.Results
                .FirstOrDefault(v => v.Type == "Trailer" && v.Site == "YouTube");

            return trailer != null ? $"https://www.youtube.com/watch?v={trailer.Key}" : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get trailer URL for TMDb ID: {TmdbId}", tmdbId);
            return null;
        }
    }

    public async Task<Movie?> GetMovieAsync(int tmdbId, CancellationToken token = default)
    {
        try
        {
            var movieInfo = await _client.GetMovieAsync(tmdbId, cancellationToken: token);
            return new Movie
            {
                TmdbId = movieInfo.Id,
                Title = movieInfo.Title,
                Year = movieInfo.ReleaseDate?.Year ?? 0,
                Overview = movieInfo.Overview,
                PosterPath = movieInfo.PosterPath
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get movie info for TMDb ID: {TmdbId}", tmdbId);
            return null;
        }
    }
}
