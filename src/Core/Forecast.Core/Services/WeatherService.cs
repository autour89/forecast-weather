using AutoMapper;
using Forecast.Core.Configuration;
using Forecast.Core.Interfaces;
using Forecast.Core.Models.DAOs;
using Forecast.Core.Models.DTOs;
using Serilog;

namespace Forecast.Core.Services;

public class WeatherService(ILogger logger, HttpClientFactory httpClientFactory, IMapper mapper)
    : IWeatherService
{
    private readonly HttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task<WeatherData?> ByCoordinatesAsync(double latitude, double longitude)
    {
        try
        {
            var endpoint = string.Format(
                AppConfiguration.ByCoordinates,
                latitude,
                longitude,
                AppConfiguration.OpenWeatherMapApiKey
            );
            var response = await _httpClientFactory.GetAsync<WeatherResponse>(endpoint);

            if (response == null)
                return null;

            return _mapper.Map<WeatherData>(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to get weather by coordinates");
            throw;
        }
    }

    public async Task<WeatherData?> ByCityAsync(string city)
    {
        try
        {
            var endpoint = string.Format(
                AppConfiguration.ByCity,
                Uri.EscapeDataString(city),
                AppConfiguration.OpenWeatherMapApiKey
            );
            var response = await _httpClientFactory.GetAsync<WeatherResponse>(endpoint);

            if (response == null)
                return null;

            return _mapper.Map<WeatherData>(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to get weather by city");
            throw;
        }
    }
}
