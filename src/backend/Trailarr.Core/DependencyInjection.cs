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
        var tmdbOptions = configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();
        services.Configure<TmdbOptions>(configuration.GetSection(TmdbOptions.Section));

        // Register TMDb services
        if (tmdbOptions?.ApiKey != null && tmdbOptions.Enabled)
        {
            services.AddSingleton<TMDbClient>(_ => new TMDbClient(tmdbOptions.ApiKey));
            services.AddSingleton<ITmdbService, TmdbService>();
        }
        else
        {
            services.AddSingleton<ITmdbService, MockTmdbService>();
        }

        // Register core services
        services.AddSingleton<ITrailarrConfigService, TrailarrConfigService>();
        services.AddSingleton<IMovieScanner, MovieScanner>();
        services.AddSingleton<IYoutubeClient, YoutubeClientWrapper>();
        services.AddSingleton<ITrailerService, TrailerService>();
        services.AddHostedService<TrailerDownloadBackgroundService>();

        return services;
    }
}
