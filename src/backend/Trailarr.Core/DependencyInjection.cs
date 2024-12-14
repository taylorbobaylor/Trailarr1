using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TMDbLib.Client;
using Trailarr.Core.Configuration;
using Trailarr.Core.Services;

namespace Trailarr.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddTrailarrCore(this IServiceCollection services, IConfiguration configuration)
    {
        var tmdbOptions = configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>()
            ?? throw new InvalidOperationException("TMDb configuration is missing");

        services.AddSingleton<TMDbClient>(_ => new TMDbClient(tmdbOptions.ApiKey));
        services.AddSingleton<ITrailarrConfigService, TrailarrConfigService>();
        services.AddSingleton<IMovieScanner, MovieScanner>();
        services.AddSingleton<ITmdbService, TmdbService>();
        services.AddSingleton<ITrailerService, TrailerService>();
        services.AddHostedService<TrailerDownloadBackgroundService>();

        return services;
    }
}
