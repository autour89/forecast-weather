namespace Forecast.Core.Interfaces;

public interface IAudioService
{
    Task PlaySuccessSound();
    Task PlayFailureSound();
}
