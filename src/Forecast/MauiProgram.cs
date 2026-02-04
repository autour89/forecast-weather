using System.Reflection;
using System.Text;
using Forecast.Core.Configuration;
using Forecast.Core.Interfaces;
using Forecast.Core.Services;
using Forecast.Services;
using Forecast.ViewModels;
using Forecast.Views;
using Microsoft.Extensions.Logging;

namespace Forecast;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.ConfigureLogging().ConfigureMauiApp();

        builder
            .Services.RegisterAutoMapper()
            .RegisterDatabase()
            .RegisterApplicationServices()
            .RegisterViewModels()
            .RegisterViews();

        return builder.Build();
    }

    private static MauiAppBuilder ConfigureLogging(this MauiAppBuilder builder)
    {
        // Keep default logging (debug) — Serilog removed
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder;
    }

    private static MauiAppBuilder ConfigureMauiApp(this MauiAppBuilder builder)
    {
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder;
    }

    private static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
    {
        // AutoMapper removed: no-op registration
        return services;
    }

    private static IServiceCollection RegisterDatabase(this IServiceCollection services)
    {
        // EF Core removed — no DB registration
        return services;
    }

    private static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<WeatherHttpClientFactory>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<ISettingsService, SettingsPersistenceService>();
        services.AddSingleton<ILocationService, LocationService>();
        services.AddSingleton<IAudioService, AudioService>();

        return services;
    }

    private static IServiceCollection RegisterViewModels(this IServiceCollection services)
    {
        services.AddTransient<WeatherViewModel>();
        return services;
    }

    private static IServiceCollection RegisterViews(this IServiceCollection services)
    {
        services.AddTransient<WeatherPage>();
        return services;
    }
}
