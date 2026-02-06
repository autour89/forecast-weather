namespace Forecast.Core.Models.DAOs;

public class WeatherData : ObservableObject
{
    private string? _cityName;
    private string? _country;
    private double _temperature;
    private double _feelsLike;
    private string? _description;
    private string? _mainWeather;
    private int _humidity;
    private double _windSpeed;
    private int _pressure;
    private string? _iconCode;
    private DateTime _lastUpdated;
    private bool _useCelsius = true;

    public string? CityName
    {
        get => _cityName;
        set => SetProperty(ref _cityName, value);
    }

    public string? Country
    {
        get => _country;
        set => SetProperty(ref _country, value);
    }

    public double Temperature
    {
        get => _temperature;
        set =>
            SetProperty(
                ref _temperature,
                value,
                () => OnPropertyChanged(nameof(TemperatureDisplay))
            );
    }

    public double FeelsLike
    {
        get => _feelsLike;
        set =>
            SetProperty(ref _feelsLike, value, () => OnPropertyChanged(nameof(FeelsLikeDisplay)));
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string? MainWeather
    {
        get => _mainWeather;
        set => SetProperty(ref _mainWeather, value);
    }

    public int Humidity
    {
        get => _humidity;
        set => SetProperty(ref _humidity, value);
    }

    public double WindSpeed
    {
        get => _windSpeed;
        set => SetProperty(ref _windSpeed, value);
    }

    public int Pressure
    {
        get => _pressure;
        set => SetProperty(ref _pressure, value);
    }

    public string? IconCode
    {
        get => _iconCode;
        set => SetProperty(ref _iconCode, value, () => OnPropertyChanged(nameof(IconUrl)));
    }

    public DateTime LastUpdated
    {
        get => _lastUpdated;
        set => SetProperty(ref _lastUpdated, value);
    }

    public bool UseCelsius
    {
        get => _useCelsius;
        set =>
            SetProperty(
                ref _useCelsius,
                value,
                () =>
                {
                    OnPropertyChanged(nameof(TemperatureDisplay));
                    OnPropertyChanged(nameof(FeelsLikeDisplay));
                }
            );
    }

    public string IconUrl =>
        !string.IsNullOrEmpty(IconCode)
            ? $"https://openweathermap.org/img/wn/{IconCode}@2x.png"
            : string.Empty;

    public string TemperatureDisplay
    {
        get
        {
            if (UseCelsius)
                return $"{Temperature:F1}°C";

            var tempF = Temperature * 9 / 5 + 32;
            return $"{tempF:F1}°F";
        }
    }

    public string FeelsLikeDisplay
    {
        get
        {
            if (UseCelsius)
                return $"Feels like {FeelsLike:F1}°C";

            var feelsLikeF = FeelsLike * 9 / 5 + 32;
            return $"Feels like {feelsLikeF:F1}°F";
        }
    }
}
