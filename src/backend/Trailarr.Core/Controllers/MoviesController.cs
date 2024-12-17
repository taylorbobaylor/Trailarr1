using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Trailarr.Core.Configuration;
using Trailarr.Core.Models;
using Trailarr.Core.Services;

namespace Trailarr.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieScanner _movieScanner;
    private readonly ITrailerService _trailerService;
    private readonly ILogger<MoviesController> _logger;
    private readonly MovieLibraryOptions _options;

    public MoviesController(
        IMovieScanner movieScanner,
        ITrailerService trailerService,
        ILogger<MoviesController> logger,
        IOptions<MovieLibraryOptions> options)
    {
        _movieScanner = movieScanner;
        _trailerService = trailerService;
        _logger = logger;
        _options = options.Value;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies(CancellationToken token = default)
    {
        try
        {
            var movies = await _movieScanner.ScanDirectoriesAsync(_options.Paths, token);

            if (!movies.Any())
            {
                _logger.LogInformation("No movies found in configured paths: {Paths}", string.Join(", ", _options.Paths));
                return Ok(Array.Empty<Movie>());
            }

            return Ok(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movies");
            return StatusCode(500, "An error occurred while getting movies");
        }
    }

    [HttpPost("{id}/download-trailer")]
    public async Task<ActionResult> DownloadTrailer(int id, CancellationToken token = default)
    {
        try
        {
            var movies = await _movieScanner.ScanDirectoriesAsync(_options.Paths, token);
            var movie = movies.FirstOrDefault(m => m.TmdbId == id);

            if (movie == null)
            {
                return NotFound($"Movie with TMDb ID {id} not found");
            }

            var success = await _trailerService.DownloadTrailerAsync(movie, token);
            if (!success)
            {
                return BadRequest($"Failed to download trailer for movie {movie.Title}");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading trailer for movie {Id}", id);
            return StatusCode(500, "An error occurred while downloading the trailer");
        }
    }

    [HttpPost("download-trailers")]
    public async Task<ActionResult> DownloadTrailers([FromBody] int[] ids, CancellationToken token = default)
    {
        try
        {
            var movies = await _movieScanner.ScanDirectoriesAsync(_options.Paths, token);
            var selectedMovies = movies.Where(m => ids.Contains(m.TmdbId ?? -1)).ToList();

            if (!selectedMovies.Any())
            {
                return NotFound("No movies found with the provided IDs");
            }

            await _trailerService.DownloadMissingTrailersAsync(selectedMovies, token);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading trailers for movies {Ids}", string.Join(", ", ids));
            return StatusCode(500, "An error occurred while downloading the trailers");
        }
    }
}
