namespace Forecast.Core.Models.Entities;

public class AppSettingsEntity : BaseEntity
{
    public bool IsDarkTheme { get; set; }
    public bool UseCelsius { get; set; }
    public string? LastSearchedCity { get; set; }
}
