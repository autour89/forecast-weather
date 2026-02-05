using Forecast.Core.Interfaces;
using Serilog;

namespace Forecast.Services;

public class LocationService(ILogger logger) : ILocationService
{
    private readonly ILogger _logger = logger;

    public async Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            _logger.Information("Location permission status: {Status}", status);

            if (status != PermissionStatus.Granted)
            {
                return null;
            }

            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10),
                }
            );

            if (location != null)
            {
                return (location.Latitude, location.Longitude);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unable to get location");
            return null;
        }
    }
}
