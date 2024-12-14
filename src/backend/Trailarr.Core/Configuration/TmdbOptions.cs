namespace Trailarr.Core.Configuration;

public class TmdbOptions
{
    public const string Section = "TMDb";
    public required string ApiKey { get; set; }
}
