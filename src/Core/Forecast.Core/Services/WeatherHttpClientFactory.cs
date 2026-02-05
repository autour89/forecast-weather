using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Forecast.Core.Configuration;
using Forecast.Core.Handlers;
using Serilog;

namespace Forecast.Core.Services;

public class WeatherHttpClientFactory
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
    private HttpClient? _httpClient;
    private readonly JsonSerializerOptions _deserializerOptions;
    private readonly Lock _lock = new();
    private readonly ILogger _logger;

    public WeatherHttpClientFactory(ILogger logger)
    {
        _logger = logger;
        _deserializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };
    }

    private HttpClient GetHttpClient()
    {
        if (_httpClient == null)
        {
            lock (_lock)
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient(new WeatherApiMessageHandler(_logger))
                    {
                        BaseAddress = new Uri(AppConfiguration.OpenWeatherMapBaseUrl),
                        Timeout = Timeout,
                    };
                    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                }
            }
        }
        return _httpClient;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint)
        where TResponse : class
    {
        var response = await GetHttpClient().GetAsync(endpoint);

        return await ProcessResponseAsync<TResponse>(response);
    }

    private async Task<TResponse?> ProcessResponseAsync<TResponse>(HttpResponseMessage httpResponse)
        where TResponse : class
    {
        var responseBody = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"HTTP {httpResponse.StatusCode}: {responseBody}");
        }

        var response =
            JsonSerializer.Deserialize<TResponse>(responseBody, _deserializerOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize response to {typeof(TResponse).Name}"
            );
        return response;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
