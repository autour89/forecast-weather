using System.Text.Json;
using System.Text.Json.Serialization;
using Forecast.Core.Configuration;
using Forecast.Core.Handlers;
using Serilog;

namespace Forecast.Core.Services;

public class HttpClientFactory
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
    private HttpClient? _httpClient;
    private readonly JsonSerializerOptions _deserializerOptions;
    private readonly Lock _lock = new();
    private readonly ILogger _logger;

    public HttpClientFactory(ILogger logger)
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
                    _httpClient = new HttpClient(new MessageHandler(_logger))
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

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
        where TRequest : class
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(request, _serializerOptions);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await GetHttpClient().PostAsync(endpoint, content);

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
