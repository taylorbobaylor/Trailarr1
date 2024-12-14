using NzbDrone.Core.Messaging.Events;
using Serilog;
using System.Text.Json;

namespace NzbDrone.Core.Configuration;

public class TrailarrConfigService(IConfigFileProvider configFileProvider) : ITrailarrConfigService
{
    private const string CONFIG_KEY = "trailarr";
    private readonly IConfigFileProvider _configFileProvider = configFileProvider;

    public TrailarrSettings GetSettings()
    {
        var json = _configFileProvider.GetValue(CONFIG_KEY, "{}");
        var settings = JsonSerializer.Deserialize<TrailarrSettings>(json) ?? new TrailarrSettings
        {
            MediaLibraryPaths = new List<string>(),
            AutoDownloadTrailers = false,
            DownloadOnAdd = true,
            DownloadMissingTrailers = false
        };

        settings.MediaLibraryPaths ??= new List<string>();
        return settings;
    }

    public void SaveSettings(TrailarrSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.MediaLibraryPaths ??= new List<string>();

        var json = JsonSerializer.Serialize(settings);
        _configFileProvider.SetValue(CONFIG_KEY, json);
        Log.Information("Saved Trailarr settings: {@Settings}", settings);
    }

    public void AddMediaLibrary(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var settings = GetSettings();
        if (!settings.MediaLibraryPaths.Contains(path))
        {
            settings.MediaLibraryPaths.Add(path);
            SaveSettings(settings);
            Log.Information("Added media library path: {Path}", path);
        }
    }

    public void RemoveMediaLibrary(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var settings = GetSettings();
        if (settings.MediaLibraryPaths.Remove(path))
        {
            SaveSettings(settings);
            Log.Information("Removed media library path: {Path}", path);
        }
    }

    public void SetAutoDownloadPreferences(bool autoDownload, bool downloadOnAdd, bool downloadMissing)
    {
        var settings = GetSettings();
        settings.AutoDownloadTrailers = autoDownload;
        settings.DownloadOnAdd = downloadOnAdd;
        settings.DownloadMissingTrailers = downloadMissing;
        SaveSettings(settings);
        Log.Information("Updated auto-download preferences: AutoDownload={AutoDownload}, DownloadOnAdd={DownloadOnAdd}, DownloadMissing={DownloadMissing}",
            autoDownload, downloadOnAdd, downloadMissing);
    }
}
