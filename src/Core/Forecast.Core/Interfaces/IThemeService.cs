namespace Forecast.Core.Interfaces;

public interface IThemeService
{
    void SetTheme(bool isDarkTheme);
    bool IsDark();
}