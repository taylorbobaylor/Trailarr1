using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Trailarr.Core.Configuration;

public class TrailarrConfigService(ILogger<TrailarrConfigService> logger) : ITrailarrConfigService
{
    private readonly string _configPath = Path.Combine(AppContext.BaseDirectory, "config", "trailarr.json");
    private readonly ILogger<TrailarrConfigService> _logger = logger;
    private TrailarrSettings? _cachedSettings;

    public TrailarrSettings GetSettings()
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        try
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath);
                _cachedSettings = JsonSerializer.Deserialize<TrailarrSettings>(json);
                if (_cachedSettings != null)
                {
                    return _cachedSettings;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings from {ConfigPath}", _configPath);
        }

        _cachedSettings = new TrailarrSettings
        {
            MediaLibraryPaths = new List<string>(),
            AutoDownloadTrailers = true,
            ScanIntervalHours = 1
        };

        SaveSettings(_cachedSettings);
        return _cachedSettings;
    }

    public void SaveSettings(TrailarrSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
            _cachedSettings = settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving settings to {ConfigPath}", _configPath);
            throw;
        }
    }
}
