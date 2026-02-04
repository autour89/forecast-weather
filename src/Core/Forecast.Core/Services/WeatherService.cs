using Forecast.Core.Configuration;
using Forecast.Core.Interfaces;
using Forecast.Core.Models.DAOs;
using Forecast.Core.Models.DTOs;

namespace Forecast.Core.Services;

public class WeatherService : IWeatherService
{
    private readonly WeatherHttpClientFactory _httpClientFactory;

    public WeatherService(
        WeatherHttpClientFactory httpClientFactory
    )
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<WeatherData?> GetWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        try
        {
            var endpoint =
                $"weather?lat={latitude}&lon={longitude}&appid={AppConfiguration.OpenWeatherMapApiKey}&units=metric";
            var response = await _httpClientFactory.GetAsync<WeatherResponse>(endpoint);

            if (response == null) return null;

            return MapToWeatherData(response);
        }
        catch
        {
            throw;
        }
    }

    public async Task<WeatherData?> GetWeatherByCityAsync(string city)
    {
        try
        {

            var endpoint =
                $"weather?q={Uri.EscapeDataString(city)}&appid={AppConfiguration.OpenWeatherMapApiKey}&units=metric";
            var response = await _httpClientFactory.GetAsync<WeatherResponse>(endpoint);

            if (response == null) return null;

            return MapToWeatherData(response);
        }
        catch
        {
            throw;
        }
    }

    private WeatherData MapToWeatherData(WeatherResponse response)
    {
        var weather = response.Weather?.FirstOrDefault();
        return new WeatherData
        {
            CityName = response.Name,
            Country = response.Sys?.Country,
            Temperature = response.Main?.Temp ?? 0,
            FeelsLike = response.Main?.FeelsLike ?? 0,
            Description = weather?.Description,
            MainWeather = weather?.Main,
            Humidity = response.Main?.Humidity ?? 0,
            WindSpeed = response.Wind?.Speed ?? 0,
            Pressure = response.Main?.Pressure ?? 0,
            IconCode = weather?.Icon,
            LastUpdated = DateTimeOffset.FromUnixTimeSeconds(response.Dt).DateTime
        };
    }
}
