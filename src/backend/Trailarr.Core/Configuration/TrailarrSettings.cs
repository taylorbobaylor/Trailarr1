namespace Trailarr.Core.Configuration;

public class TrailarrSettings
{
    public required List<string> MediaLibraryPaths { get; set; } = new();
    public bool AutoDownloadTrailers { get; set; } = true;
    public int ScanIntervalHours { get; set; } = 1;
}
