using System;

namespace NzbDrone.Common.Http;

public interface IHttpRequestBuilderFactory
{
    HttpRequestBuilder Create();
    HttpRequestBuilder CreateBuilder(string baseUrl);
}

public class HttpRequestBuilderFactory : IHttpRequestBuilderFactory
{
    private readonly HttpRequestBuilder _rootBuilder;

    public HttpRequestBuilderFactory(HttpRequestBuilder rootBuilder)
    {
        _rootBuilder = rootBuilder ?? throw new ArgumentNullException(nameof(rootBuilder));
    }

    public HttpRequestBuilder Create()
    {
        return _rootBuilder.Clone();
    }

    public HttpRequestBuilder CreateBuilder(string baseUrl)
    {
        return new HttpRequestBuilder(baseUrl);
    }
}
