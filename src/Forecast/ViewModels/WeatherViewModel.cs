using Forecast.Core.Interfaces;
using Forecast.Core.Models.DAOs;
using Forecast.Utilities;
using Serilog;

namespace Forecast.ViewModels;

public class WeatherViewModel : BaseViewModel<WeatherData>
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly IAudioService _audioService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger _logger;

    private string _searchCity = string.Empty;
    private bool _isRefreshing;
    private string _temperatureDisplay = string.Empty;
    private string _feelsLikeDisplay = string.Empty;
    private string _description = string.Empty;
    private string _cityName = string.Empty;
    private bool _isDarkTheme;
    private bool _useCelsius = true;

    public WeatherViewModel(
        IWeatherService weatherService,
        ILocationService locationService,
        IAudioService audioService,
        ISettingsService settingsService,
        ILogger logger
    )
    {
        _weatherService = weatherService;
        _locationService = locationService;
        _audioService = audioService;
        _settingsService = settingsService;
        _logger = logger;

        SearchWeatherCommand = new AsyncCommand(SearchWeatherAsync);
        GetCurrentLocationWeatherCommand = new AsyncCommand(GetCurrentLocationWeatherAsync);
        RefreshCommand = new AsyncCommand(RefreshWeatherAsync);

        _ = InitializeAsync();
    }

    #region Properties

    public string SearchCity
    {
        get => _searchCity;
        set => SetProperty(ref _searchCity, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public string TemperatureDisplay
    {
        get => _temperatureDisplay;
        set => SetProperty(ref _temperatureDisplay, value);
    }

    public string FeelsLikeDisplay
    {
        get => _feelsLikeDisplay;
        set => SetProperty(ref _feelsLikeDisplay, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string CityName
    {
        get => _cityName;
        set => SetProperty(ref _cityName, value);
    }

    public string IconUrl => Data?.IconUrl ?? string.Empty;

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                ApplyTheme(value);
                _ = _settingsService.SetIsDarkThemeAsync(value);
            }
        }
    }

    public bool UseCelsius
    {
        get => _useCelsius;
        set
        {
            if (SetProperty(ref _useCelsius, value))
            {
                UpdateTemperatureDisplay();
                _ = _settingsService.SetUseCelsiusAsync(value);
            }
        }
    }

    public bool HasWeatherData => Data != null;

    #endregion

    #region Commands

    public AsyncCommand SearchWeatherCommand { get; }
    public AsyncCommand GetCurrentLocationWeatherCommand { get; }
    public AsyncCommand RefreshCommand { get; }

    #endregion

    #region Initialization

    private async Task InitializeAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            _logger.Information("WeatherViewModel initialization started");

            await _settingsService.InitializeAsync();

            var lastCity = await _settingsService.GetLastSearchedCityAsync();
            if (!string.IsNullOrEmpty(lastCity))
            {
                SearchCity = lastCity;
            }

            IsDarkTheme = await _settingsService.GetIsDarkThemeAsync();
            ApplyTheme(IsDarkTheme);

            UseCelsius = await _settingsService.GetUseCelsiusAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during WeatherViewModel initialization");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Command Implementations

    private async Task SearchWeatherAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchCity))
        {
            ErrorMessage = "Please enter a city name";
            return;
        }

        ErrorMessage = string.Empty;

        try
        {
            var weatherData = await _weatherService.GetWeatherByCityAsync(SearchCity);

            if (weatherData != null)
            {
                UpdateWeatherData(weatherData);
                await _settingsService.SetLastSearchedCityAsync(SearchCity);
                await _audioService.PlaySuccessSound();
            }
            else
            {
                ErrorMessage = "City not found";
                await _audioService.PlayFailureSound();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to fetch weather: {ex.Message}";
            await _audioService.PlayFailureSound();
        }
    }

    private async Task GetCurrentLocationWeatherAsync()
    {
        ErrorMessage = string.Empty;

        try
        {
            var location = await _locationService.GetCurrentLocationAsync();

            if (location.HasValue)
            {
                var weatherData = await _weatherService.GetWeatherByCoordinatesAsync(
                    location.Value.Latitude,
                    location.Value.Longitude
                );

                if (weatherData != null)
                {
                    UpdateWeatherData(weatherData);
                    SearchCity = weatherData.CityName ?? string.Empty;
                    await _audioService.PlaySuccessSound();
                }
                else
                {
                    ErrorMessage = "Unable to fetch weather for your location";
                    await _audioService.PlayFailureSound();
                }
            }
            else
            {
                ErrorMessage = "Unable to get current location. Please check permissions.";
                await _audioService.PlayFailureSound();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to fetch weather: {ex.Message}";
            await _audioService.PlayFailureSound();
        }
    }

    private async Task RefreshWeatherAsync()
    {
        IsRefreshing = true;

        try
        {
            if (!string.IsNullOrEmpty(SearchCity))
            {
                await SearchWeatherAsync();
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    #endregion

    #region Helper Methods

    private void UpdateWeatherData(WeatherData weatherData)
    {
        Data = weatherData;
        CityName = weatherData.CityName ?? string.Empty;
        Description = weatherData.Description ?? string.Empty;
        OnPropertyChanged(nameof(IconUrl));
        OnPropertyChanged(nameof(HasWeatherData));
        UpdateTemperatureDisplay();
    }

    private void UpdateTemperatureDisplay()
    {
        if (Data == null)
        {
            TemperatureDisplay = string.Empty;
            FeelsLikeDisplay = string.Empty;
            return;
        }

        if (UseCelsius)
        {
            TemperatureDisplay = $"{Data.Temperature:F1}°C";
            FeelsLikeDisplay = $"Feels like {Data.FeelsLike:F1}°C";
        }
        else
        {
            var tempF = Data.Temperature * 9 / 5 + 32;
            var feelsLikeF = Data.FeelsLike * 9 / 5 + 32;
            TemperatureDisplay = $"{tempF:F1}°F";
            FeelsLikeDisplay = $"Feels like {feelsLikeF:F1}°F";
        }
    }

    private void ApplyTheme(bool isDark)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current?.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error applying theme");
        }
    }

    #endregion
}
