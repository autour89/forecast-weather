using Forecast.Core.Models.DAOs;

namespace Forecast.Core.Interfaces;

public interface IWeatherService
{
    Task<WeatherData?> GetWeatherByCoordinatesAsync(double latitude, double longitude);
    Task<WeatherData?> GetWeatherByCityAsync(string city);
}
