namespace NzbDrone.Core.Configuration;

public class TrailarrSettings
{
    public List<string> MediaLibraryPaths { get; set; } = new();
    public bool AutoDownloadTrailers { get; set; }
    public bool DownloadOnAdd { get; set; } = true;
    public bool DownloadMissingTrailers { get; set; }
}
