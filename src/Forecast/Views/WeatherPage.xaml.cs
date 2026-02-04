using Forecast.ViewModels;

namespace Forecast.Views;

public partial class WeatherPage : ContentPage
{
    private readonly WeatherViewModel _viewModel;

    public WeatherPage(WeatherViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        Appearing += (s, e) => { _ = _viewModel.InitializeAsync(); };
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.ToggleThemeCommand.Execute(null);
    }

    private void OnTemperatureUnitToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.ToggleTemperatureUnitCommand.Execute(null);
    }
}
