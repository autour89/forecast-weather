namespace Forecast.Core.Handlers;

public class WeatherApiMessageHandler : DelegatingHandler
{
    public WeatherApiMessageHandler()
    {
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            await LogExchangeAsync(request, response);

            return response;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task LogExchangeAsync(HttpRequestMessage request, HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
    }
}
