namespace Trailarr.Core.Configuration;

public class TmdbOptions
{
    public const string Section = "TMDb";
    public string? ApiKey { get; set; }
    public bool Enabled { get; set; }
}
