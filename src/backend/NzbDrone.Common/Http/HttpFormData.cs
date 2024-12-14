namespace NzbDrone.Common.Http;

public class HttpFormData
{
    public required string Name { get; set; }
    public required string FileName { get; set; }
    public required byte[] ContentData { get; set; }
    public required string ContentType { get; set; }
}
