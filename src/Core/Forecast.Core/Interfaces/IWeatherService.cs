using Forecast.Core.Models.DAOs;

namespace Forecast.Core.Interfaces;

public interface IWeatherService
{
    Task<WeatherData?> ByCoordinatesAsync(double latitude, double longitude);
    Task<WeatherData?> ByCityAsync(string city);
}
