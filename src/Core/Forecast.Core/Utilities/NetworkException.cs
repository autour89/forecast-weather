namespace Forecast.Core.Utilities;

public class NetworkException : Exception
{
    public NetworkException()
        : base("No internet connection available. Please check your network settings.") { }

    public NetworkException(string message)
        : base(message) { }

    public NetworkException(string message, Exception innerException)
        : base(message, innerException) { }
}
