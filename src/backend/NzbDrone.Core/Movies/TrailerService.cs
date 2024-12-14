using System.Text.RegularExpressions;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.MediaFiles;
using Serilog;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace NzbDrone.Core.Movies;

public interface ITrailerService
{
    Task DownloadTrailerAsync(Movie movie, CancellationToken token = default);
    Task DownloadMissingTrailersAsync(CancellationToken token = default);
    bool HasTrailer(Movie movie);
}

public class TrailerService(ITrailarrConfigService configService,
                          IMovieService movieService,
                          ITmdbService tmdbService,
                          IDiskProvider diskProvider) : ITrailerService
{
    private readonly YoutubeClient _youtube = new();
    private static readonly Regex YoutubeIdRegex = new(@"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})", RegexOptions.Compiled);

    public async Task DownloadTrailerAsync(Movie movie, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(movie);
        ArgumentNullException.ThrowIfNull(movie.FilePath);

        var trailerPath = GetTrailerPath(movie.FilePath);
        if (File.Exists(trailerPath))
        {
            Log.Debug("Trailer already exists for movie: {Title}", movie.Title);
            return;
        }

        var trailerUrl = await tmdbService.GetTrailerUrlAsync(movie.TmdbId, token);
        if (string.IsNullOrEmpty(trailerUrl))
        {
            Log.Warning("No trailer URL found for movie: {Title}", movie.Title);
            return;
        }

        var match = YoutubeIdRegex.Match(trailerUrl);
        if (!match.Success)
        {
            Log.Warning("Invalid YouTube URL for movie: {Title}", movie.Title);
            return;
        }

        var videoId = match.Groups[1].Value;

        try
        {
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId, token);
            var streamInfo = streamManifest.GetMuxedStreams()
                                         .OrderByDescending(s => s.VideoQuality)
                                         .FirstOrDefault();

            if (streamInfo == null)
            {
                Log.Error("No suitable stream found for movie trailer: {Title}", movie.Title);
                return;
            }

            await using var stream = await _youtube.Videos.Streams.GetAsync(streamInfo, token);
            await using var fileStream = File.Create(trailerPath);
            await stream.CopyToAsync(fileStream, token);

            Log.Information("Downloaded trailer for movie: {Title}", movie.Title);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to download trailer for movie: {Title}", movie.Title);
            if (File.Exists(trailerPath))
            {
                File.Delete(trailerPath);
            }
        }
    }

    public async Task DownloadMissingTrailersAsync(CancellationToken token = default)
    {
        var settings = configService.GetSettings();
        if (!settings.AutoDownloadTrailers)
        {
            return;
        }

        var movies = await movieService.GetAllMoviesAsync();
        foreach (var movie in movies.Where(m => !HasTrailer(m)))
        {
            await DownloadTrailerAsync(movie, token);
        }
    }

    public bool HasTrailer(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);
        ArgumentNullException.ThrowIfNull(movie.FilePath);

        return File.Exists(GetTrailerPath(movie.FilePath));
    }

    private static string GetTrailerPath(string moviePath)
    {
        var directory = Path.GetDirectoryName(moviePath) ?? throw new ArgumentException("Invalid movie path", nameof(moviePath));
        var fileName = Path.GetFileNameWithoutExtension(moviePath);
        var extension = Path.GetExtension(moviePath);
        return Path.Combine(directory, $"{fileName}-trailer{extension}");
    }
}
