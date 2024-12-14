using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Trailarr.Core.Models;

namespace Trailarr.Core.Services;

public partial class MovieScanner(ITmdbService tmdbService, ILogger<MovieScanner> logger) : IMovieScanner
{
    private readonly ITmdbService _tmdbService = tmdbService;
    private readonly ILogger<MovieScanner> _logger = logger;
    private static readonly string[] VideoExtensions = { ".mp4", ".mkv", ".avi", ".m4v", ".mov" };

    [GeneratedRegex(@"^(?<title>.+?)(?:\s*\((?<year>\d{4})\))?$", RegexOptions.IgnoreCase)]
    private static partial Regex MoviePattern();

    public async Task<IEnumerable<Movie>> ScanDirectoryAsync(string path, CancellationToken token = default)
    {
        var movies = new List<Movie>();
        try
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(f => VideoExtensions.Contains(Path.GetExtension(f).ToLower()));

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var match = MoviePattern().Match(fileName);

                if (!match.Success)
                {
                    _logger.LogWarning("Could not parse movie title from filename: {FileName}", fileName);
                    continue;
                }

                var title = match.Groups["title"].Value.Trim();
                var yearString = match.Groups["year"].Value;
                int? year = !string.IsNullOrEmpty(yearString) ? int.Parse(yearString) : null;

                var movie = await _tmdbService.GetMovieAsync(title, year, token);
                if (movie == null)
                {
                    _logger.LogWarning("Could not find movie in TMDb: {Title} ({Year})", title, year);
                    continue;
                }

                movie.FilePath = file;
                movies.Add(movie);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning directory: {Path}", path);
        }

        return movies;
    }

    public async Task<IEnumerable<Movie>> ScanDirectoriesAsync(IEnumerable<string> paths, CancellationToken token = default)
    {
        var tasks = paths.Select(path => ScanDirectoryAsync(path, token));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(x => x);
    }
}
