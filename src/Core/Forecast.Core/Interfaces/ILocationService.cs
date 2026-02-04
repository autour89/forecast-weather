namespace Forecast.Core.Interfaces;

public interface ILocationService
{
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
}
