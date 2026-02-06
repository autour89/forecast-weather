using AutoMapper;
using Forecast.Core.Configuration;
using Forecast.Core.Interfaces;
using Forecast.Core.Mappings;
using Forecast.Core.Services;
using Forecast.Core.ViewModels;
using Forecast.Services;
using Forecast.Views;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using Serilog;
using Encoding = System.Text.Encoding;

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
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews();

        return builder.Build();
    }

    private static MauiAppBuilder ConfigureLogging(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<Serilog.ILogger>(provider =>
        {
            var logPath = Path.Combine(FileSystem.AppDataDirectory, "Logs");
            Directory.CreateDirectory(logPath);

            return new LoggerConfiguration()
                .WriteTo.File(
                    path: Path.Combine(logPath, AppConfiguration.LogFileName),
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u4}] {Message:lj}{NewLine}{Exception}",
                    fileSizeLimitBytes: AppConfiguration.LogFileSizeLimitBytes,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: AppConfiguration.LogRetainedFileCountLimit,
                    encoding: Encoding.UTF8
                )
                .CreateLogger();
        });
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
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        config.AssertConfigurationIsValid();
        services.AddSingleton(config.CreateMapper());
        return services;
    }

    private static IServiceCollection RegisterDatabase(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<HttpClientFactory>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddSingleton<ILocationService, LocationService>();
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton(AudioManager.Current);

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
