namespace NzbDrone.Core.Configuration;

public interface ITrailarrConfigService
{
    TrailarrSettings GetSettings();
    void SaveSettings(TrailarrSettings settings);
    void AddMediaLibrary(string path);
    void RemoveMediaLibrary(string path);
    void SetAutoDownloadPreferences(bool autoDownload, bool downloadOnAdd, bool downloadMissing);
}
