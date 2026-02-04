using Forecast.Core.Interfaces;

namespace Forecast.Services;

public class AudioService : IAudioService
{
    public async Task PlaySuccessSound()
    {
        try
        {
            // Note: Audio files need to be added to Resources/Raw folder
            // For now, we'll use a vibration pattern as feedback
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));

            // TODO: Uncomment when audio files are added
            // var stream = await FileSystem.OpenAppPackageFileAsync("success.mp3");
            // var player = AudioManager.Current.CreatePlayer(stream);
            // player.Play();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to play success sound: {ex.Message}");
        }
    }

    public async Task PlayFailureSound()
    {
        try
        {
            // Note: Audio files need to be added to Resources/Raw folder
            // For now, we'll use a vibration pattern as feedback
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));

            // TODO: Uncomment when audio files are added
            // var stream = await FileSystem.OpenAppPackageFileAsync("failure.mp3");
            // var player = AudioManager.Current.CreatePlayer(stream);
            // player.Play();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to play failure sound: {ex.Message}");
        }

        await Task.CompletedTask;
    }
}
