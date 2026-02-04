namespace Forecast.Core.Models;

public class AppSettings
{
    public bool IsDarkTheme { get; set; }
    public bool UseCelsius { get; set; } = true;
    public string? LastSearchedCity { get; set; }
}
