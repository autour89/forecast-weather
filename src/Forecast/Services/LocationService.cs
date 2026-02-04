using Forecast.Core.Interfaces;

namespace Forecast.Services;

public class LocationService : ILocationService
{
    public async Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

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
            System.Diagnostics.Debug.WriteLine($"Unable to get location: {ex.Message}");
            return null;
        }
    }
}
