using Forecast.Core.Interfaces;
using Plugin.Maui.Audio;
using Serilog;

namespace Forecast.Services;

public class AudioService(ILogger logger, IAudioManager audioManager) : IAudioService
{
    private const string Success = "success.mp3";
    private const string Failure = "failure.mp3";
    private readonly ILogger _logger = logger;
    private readonly IAudioManager _audioManager = audioManager;

    public async Task PlaySuccessSound()
    {
        try
        {
            await PlaySound(Success, hapticFeedback: true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to play success sound");
        }
    }

    public async Task PlayFailureSound()
    {
        try
        {
            await PlaySound(Failure, hapticFeedback: true, vibrationDurationMs: 500);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to play failure sound");
        }
    }

    private async Task PlaySound(
        string fileName,
        bool hapticFeedback = true,
        int vibrationDurationMs = 100
    )
    {
        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            var audioPlayer = _audioManager.CreatePlayer(stream);
            audioPlayer.Volume = 0.7;
            audioPlayer.Play();
            if (hapticFeedback)
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(vibrationDurationMs));
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to play sound: {fileName}");
        }
    }
}
