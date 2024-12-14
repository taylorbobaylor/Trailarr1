namespace Trailarr.Core.Configuration;

public interface ITrailarrConfigService
{
    TrailarrSettings GetSettings();
    void SaveSettings(TrailarrSettings settings);
}
