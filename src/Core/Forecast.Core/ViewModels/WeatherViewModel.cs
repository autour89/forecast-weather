using Forecast.Core.Interfaces;
using Forecast.Core.Models.DAOs;
using Forecast.Core.Utilities;
using Forecast.Utilities;
using Microsoft.Maui.Networking;
using Serilog;

namespace Forecast.Core.ViewModels;

public class WeatherViewModel : BaseViewModel<WeatherData>
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly IAudioService _audioService;
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;
    private readonly ILogger _logger;

    private string _searchCity = string.Empty;
    private bool _isDarkTheme;

    public WeatherViewModel(
        IWeatherService weatherService,
        ILocationService locationService,
        IAudioService audioService,
        ISettingsService settingsService,
        IThemeService themeService,
        ILogger logger
    )
    {
        _weatherService = weatherService;
        _locationService = locationService;
        _audioService = audioService;
        _settingsService = settingsService;
        _themeService = themeService;
        _logger = logger;

        SearchCommand = new AsyncCommand(SearchAsync);
        CurrentLocationCommand = new AsyncCommand(CurrentLocationAsync);
        RefreshCommand = new AsyncCommand(RefreshWeatherAsync);

        _ = InitializeAsync();
    }

    #region Properties

    public string SearchCity
    {
        get => _searchCity;
        set => SetProperty(ref _searchCity, value);
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                _themeService.SetTheme(value);
                _ = _settingsService.SetIsDarkThemeAsync(value);
            }
        }
    }

    public bool HasWeatherData => Data != null;

    #endregion

    #region Commands

    public AsyncCommand SearchCommand { get; }
    public AsyncCommand CurrentLocationCommand { get; }
    public AsyncCommand RefreshCommand { get; }

    #endregion

    #region Initialization

    public override async Task InitializeAsync()
    {
        try
        {
            _logger.Information("WeatherViewModel initialization started");

            await _settingsService.InitializeAsync();

            var lastCity = await _settingsService.GetLastSearchedCityAsync();
            SearchCity = !string.IsNullOrEmpty(lastCity) ? lastCity : string.Empty;

            IsDarkTheme = await _settingsService.GetIsDarkThemeAsync();
            _themeService.SetTheme(IsDarkTheme);

            if (!string.IsNullOrEmpty(lastCity))
            {
                await SearchAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during WeatherViewModel initialization");
        }
    }

    #endregion

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchCity))
        {
            ErrorMessage = "Please enter a city name";
            return;
        }

        ErrorMessage = string.Empty;

        try
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new NetworkException();
            }

            var weatherData = await _weatherService.GetWeatherByCityAsync(SearchCity);

            if (weatherData != null)
            {
                Data = weatherData;
                Data.UseCelsius = await _settingsService.GetUseCelsiusAsync();
                OnPropertyChanged(nameof(HasWeatherData));

                await _settingsService.SetLastSearchedCityAsync(SearchCity);
                await _audioService.PlaySuccessSound();
            }
            else
            {
                ErrorMessage = "City not found";
                await _audioService.PlayFailureSound();
            }
        }
        catch (NetworkException)
        {
            _logger.Warning(
                "No internet connection when trying to search weather for city: {City}",
                SearchCity
            );
            ErrorMessage = "No internet connection. Please check your network and try again.";
            await _audioService.PlayFailureSound();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error searching weather");
            ErrorMessage = $"Failed to fetch weather: {ex.Message}";
            await _audioService.PlayFailureSound();
        }
    }

    private async Task CurrentLocationAsync()
    {
        ErrorMessage = string.Empty;

        try
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new NetworkException();
            }

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
                    Data.UseCelsius = await _settingsService.GetUseCelsiusAsync();
                    SearchCity = weatherData.CityName ?? string.Empty;
                    OnPropertyChanged(nameof(HasWeatherData));

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
        catch (NetworkException)
        {
            _logger.Warning("No internet connection when trying to get current location weather");
            ErrorMessage = "No internet connection. Please check your network and try again.";
            await _audioService.PlayFailureSound();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting current location weather");
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
                await SearchAsync();
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    public async Task ToggleTemperatureUnitAsync()
    {
        if (Data != null)
        {
            Data.UseCelsius = !Data.UseCelsius;
            await _settingsService.SetUseCelsiusAsync(Data.UseCelsius);
        }
    }
}
