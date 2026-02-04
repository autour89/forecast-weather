namespace Forecast.Core.Interfaces;

public interface ISettingsService
{
    Task InitializeAsync();
    Task<bool> GetIsDarkThemeAsync();
    Task SetIsDarkThemeAsync(bool value);
    Task<bool> GetUseCelsiusAsync();
    Task SetUseCelsiusAsync(bool value);
    Task<string> GetLastSearchedCityAsync();
    Task SetLastSearchedCityAsync(string value);
}
