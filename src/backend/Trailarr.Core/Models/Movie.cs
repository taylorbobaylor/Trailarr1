namespace Trailarr.Core.Models;

public class Movie
{
    public required string Title { get; set; }
    public required string FilePath { get; set; }
    public int? Year { get; set; }
    public int? TmdbId { get; set; }
    public string? TrailerPath { get; set; }
    public bool HasTrailer => !string.IsNullOrEmpty(TrailerPath);
}
