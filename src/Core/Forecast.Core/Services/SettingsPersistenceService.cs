using Forecast.Core.Interfaces;

namespace Forecast.Core.Services;

public class SettingsPersistenceService : ISettingsService
{
    private bool _isDarkTheme = false;
    private bool _useCelsius = true;
    private string _lastSearchedCity = string.Empty;

    public SettingsPersistenceService() { }

    public Task InitializeAsync()
    {
        // In-memory settings (EF Core removed)
        return Task.CompletedTask;
    }

    public Task<bool> GetIsDarkThemeAsync()
    {
        return Task.FromResult(_isDarkTheme);
    }

    public Task SetIsDarkThemeAsync(bool value)
    {
        _isDarkTheme = value;
        return Task.CompletedTask;
    }

    public Task<bool> GetUseCelsiusAsync()
    {
        return Task.FromResult(_useCelsius);
    }

    public Task SetUseCelsiusAsync(bool value)
    {
        _useCelsius = value;
        return Task.CompletedTask;
    }

    public Task<string> GetLastSearchedCityAsync()
    {
        return Task.FromResult(_lastSearchedCity);
    }

    public Task SetLastSearchedCityAsync(string value)
    {
        _lastSearchedCity = value;
        return Task.CompletedTask;
    }
}
