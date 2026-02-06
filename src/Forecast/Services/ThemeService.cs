using Forecast.Core.Interfaces;

namespace Forecast.Services;

public class ThemeService : IThemeService
{
    public void SetTheme(bool isDarkTheme)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current?.UserAppTheme = isDarkTheme ? AppTheme.Dark : AppTheme.Light;
        });
    }

    public bool IsDark() => Application.Current?.UserAppTheme == AppTheme.Dark;
}
