namespace Forecast.Core.Models.DAOs;

public class WeatherData
{
    public string? CityName { get; set; }
    public string? Country { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public string? Description { get; set; }
    public string? MainWeather { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public int Pressure { get; set; }
    public string? IconCode { get; set; }
    public DateTime LastUpdated { get; set; }

    public string IconUrl =>
        !string.IsNullOrEmpty(IconCode)
            ? $"https://openweathermap.org/img/wn/{IconCode}@2x.png"
            : string.Empty;
}
