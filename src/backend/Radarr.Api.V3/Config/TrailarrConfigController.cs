using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Configuration;
using Radarr.Http;

namespace Radarr.Api.V3.Config;

[V3ApiController("config/trailarr")]
public class TrailarrConfigController : Controller
{
    private readonly ITrailarrConfigService _configService;

    public TrailarrConfigController(ITrailarrConfigService configService)
    {
        _configService = configService;
    }

    [HttpGet]
    public ActionResult<TrailarrSettings> GetConfig()
    {
        return Ok(_configService.GetSettings());
    }

    [HttpPut]
    public ActionResult<TrailarrSettings> SaveConfig([FromBody] TrailarrSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _configService.SaveSettings(settings);
        return Ok(settings);
    }

    [HttpPost("library")]
    public ActionResult AddLibrary([FromBody] string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        _configService.AddMediaLibrary(path);
        return Ok();
    }

    [HttpDelete("library")]
    public ActionResult RemoveLibrary([FromBody] string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        _configService.RemoveMediaLibrary(path);
        return Ok();
    }

    [HttpPut("autodownload")]
    public ActionResult SetAutoDownload([FromBody] AutoDownloadPreferences prefs)
    {
        ArgumentNullException.ThrowIfNull(prefs);
        _configService.SetAutoDownloadPreferences(prefs.AutoDownload, prefs.DownloadOnAdd, prefs.DownloadMissing);
        return Ok();
    }

    public class AutoDownloadPreferences
    {
        public bool AutoDownload { get; set; }
        public bool DownloadOnAdd { get; set; }
        public bool DownloadMissing { get; set; }
    }
}
