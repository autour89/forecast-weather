using System.Windows.Input;
using Forecast.Core.Interfaces;
using Forecast.Core.Models.DAOs;
using Forecast.Utilities;

namespace Forecast.ViewModels;

public class WeatherViewModel : BaseViewModel<WeatherData>
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly IAudioService _audioService;
    private readonly ISettingsService _settingsService;

    private string _searchCity = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isRefreshing;
    private string _temperatureDisplay = string.Empty;
    private string _feelsLikeDisplay = string.Empty;
    private bool _isDarkTheme;
    private bool _useCelsius = true;

    public WeatherViewModel(
        IWeatherService weatherService,
        ILocationService locationService,
        IAudioService audioService,
        ISettingsService settingsService
    )
    {
        _weatherService = weatherService;
        _locationService = locationService;
        _audioService = audioService;
        _settingsService = settingsService;

        SearchWeatherCommand = new AsyncCommand(SearchWeatherAsync, () => !IsBusy);
        GetCurrentLocationWeatherCommand = new AsyncCommand(GetCurrentLocationWeatherAsync, () => !IsBusy);
        RefreshCommand = new AsyncCommand(RefreshWeatherAsync);
        ToggleThemeCommand = new AsyncCommand(ToggleThemeAsync);
        ToggleTemperatureUnitCommand = new AsyncCommand(ToggleTemperatureUnitAsync);
    }

    public async Task InitializeAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            await _settingsService.InitializeAsync();

            var lastCity = await _settingsService.GetLastSearchedCityAsync();
            if (!string.IsNullOrEmpty(lastCity))
            {
                SearchCity = lastCity;
            }

            IsDarkTheme = await _settingsService.GetIsDarkThemeAsync();
            UseCelsius = await _settingsService.GetUseCelsiusAsync();

            Application.Current!.UserAppTheme = IsDarkTheme ? AppTheme.Dark : AppTheme.Light;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public string SearchCity
    {
        get => _searchCity;
        set => SetProperty(ref _searchCity, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
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

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set => SetProperty(ref _isDarkTheme, value);
    }

    public bool UseCelsius
    {
        get => _useCelsius;
        set => SetProperty(ref _useCelsius, value);
    }

    public ICommand SearchWeatherCommand { get; }
    public ICommand GetCurrentLocationWeatherCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ToggleThemeCommand { get; }
    public ICommand ToggleTemperatureUnitCommand { get; }

    private async Task SearchWeatherAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchCity))
        {
            ErrorMessage = "Please enter a city name";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var weatherData = await _weatherService.GetWeatherByCityAsync(SearchCity);

            if (weatherData != null)
            {
                Data = weatherData;
                UpdateTemperatureDisplay();
                await _settingsService.SetLastSearchedCityAsync(SearchCity);
                await _audioService.PlaySuccessSound();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to fetch weather: {ex.Message}";
            await _audioService.PlayFailureSound();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetCurrentLocationWeatherAsync()
    {
        IsBusy = true;
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
                    Data = weatherData;
                    SearchCity = weatherData.CityName ?? string.Empty;
                    UpdateTemperatureDisplay();
                    await _audioService.PlaySuccessSound();
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
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshWeatherAsync()
    {
        IsRefreshing = true;

        try
        {
            if (Data != null && !string.IsNullOrEmpty(SearchCity))
            {
                await SearchWeatherAsync();
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task ToggleThemeAsync()
    {
        IsBusy = true;
        try
        {
            var currentTheme = await _settingsService.GetIsDarkThemeAsync();
            var newTheme = !currentTheme;
            await _settingsService.SetIsDarkThemeAsync(newTheme);
            IsDarkTheme = newTheme;

            Application.Current!.UserAppTheme = newTheme ? AppTheme.Dark : AppTheme.Light;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ToggleTemperatureUnitAsync()
    {
        IsBusy = true;
        try
        {
            var currentUnit = await _settingsService.GetUseCelsiusAsync();
            var newUnit = !currentUnit;
            await _settingsService.SetUseCelsiusAsync(newUnit);
            UseCelsius = newUnit;
            UpdateTemperatureDisplay();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateTemperatureDisplay()
    {
        if (Data == null)
            return;

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
}
