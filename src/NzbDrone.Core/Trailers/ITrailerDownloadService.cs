using System.Threading.Tasks;

namespace NzbDrone.Core.Trailers
{
    public interface ITrailerDownloadService
    {
        Task<bool> DownloadTrailerAsync(string movieTitle, string trailerId, string outputPath);
        Task<string> GetBestTrailerUrlAsync(string movieTitle);
    }
}
