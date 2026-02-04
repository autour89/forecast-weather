using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forecast.Utilities;

// Minimal AsyncCommand: Execute schedules the task and captures exceptions.
public sealed class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter)
    {
        // Start the task and observe exceptions; do not use async void.
        _ = ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        try
        {
            await _execute().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AsyncCommand exception: {ex}");
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
