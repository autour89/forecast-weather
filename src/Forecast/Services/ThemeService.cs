using Forecast.Core.Interfaces;

namespace Forecast.Services;

public class ThemeService : IThemeService
{
    public void SetTheme(bool isDarkTheme)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current?.UserAppTheme = isDarkTheme ? AppTheme.Dark : AppTheme.Light;
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
        }
    }

    public bool IsDark() => Application.Current?.UserAppTheme == AppTheme.Dark;
}
