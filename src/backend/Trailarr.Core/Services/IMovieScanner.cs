using Trailarr.Core.Models;

namespace Trailarr.Core.Services;

public interface IMovieScanner
{
    Task<IEnumerable<Movie>> ScanDirectoryAsync(string path, CancellationToken token = default);
    Task<IEnumerable<Movie>> ScanDirectoriesAsync(IEnumerable<string> paths, CancellationToken token = default);
}
