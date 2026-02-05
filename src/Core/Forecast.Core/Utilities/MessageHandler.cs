using Serilog;

namespace Forecast.Core.Handlers;

public class MessageHandler : DelegatingHandler
{
    private readonly ILogger _logger;

    public MessageHandler(ILogger logger)
    {
        _logger = logger;
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
            _logger.Error(ex, "Error sending HTTP request");
            throw;
        }
    }

    private async Task LogExchangeAsync(HttpRequestMessage request, HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        _logger.Information(
            "HTTP {Method} {RequestUri} => {StatusCode}\nResponse Body: {ResponseBody}",
            request.Method,
            request.RequestUri,
            (int)response.StatusCode,
            responseBody
        );
    }
}
