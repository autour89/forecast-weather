namespace Forecast.Core.Configuration;

public static class AppConfiguration
{
    public const string OpenWeatherMapApiKey = "61b6dc08863fa4a923a7f71b62988053";
    public const string OpenWeatherMapBaseUrl = "https://api.openweathermap.org/data/2.5/";
    public const string DatabaseFileName = "forecast.db";
    public const string LogFileName = "Forecast.log";
    public const long LogFileSizeLimitBytes = 1024 * 1024 * 50;
    public const int LogRetainedFileCountLimit = 31;
    public const string ByCoordinates = "weather?lat={0}&lon={1}&appid={2}&units=metric";
    public const string ByCity = "weather?q={0}&appid={1}&units=metric";
}
