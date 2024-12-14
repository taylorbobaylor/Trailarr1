using Microsoft.Extensions.Logging;
using TMDbLib.Client;
using Trailarr.Core.Models;

namespace Trailarr.Core.Services;

public class TmdbService(TMDbClient client, ILogger<TmdbService> logger) : ITmdbService
{
    private readonly TMDbClient _client = client;
    private readonly ILogger<TmdbService> _logger = logger;

    public async Task<string?> GetTrailerUrlAsync(int tmdbId, CancellationToken token = default)
    {
        try
        {
            var movie = await _client.GetMovieAsync(tmdbId, TMDbLib.Objects.Movies.MovieMethods.Videos, token);
            if (movie?.Videos?.Results == null)
            {
                return null;
            }

            var trailer = movie.Videos.Results
                .FirstOrDefault(v => v.Type == "Trailer" && v.Site == "YouTube");

            return trailer?.Key != null ? $"https://www.youtube.com/watch?v={trailer.Key}" : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trailer URL for movie {TmdbId}", tmdbId);
            return null;
        }
    }

    public async Task<Movie?> GetMovieAsync(string title, int? year = null, CancellationToken token = default)
    {
        try
        {
            var searchResult = await _client.SearchMovieAsync(title, includeAdult: false, year: year ?? 0, cancellationToken: token);
            var movieResult = searchResult?.Results?.FirstOrDefault();
            if (movieResult == null)
            {
                return null;
            }

            return new Movie
            {
                Title = movieResult.Title,
                FilePath = string.Empty,
                Year = movieResult.ReleaseDate?.Year,
                TmdbId = movieResult.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for movie {Title} ({Year})", title, year);
            return null;
        }
    }
}
