using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Forecast.Utilities;

/// <summary>
/// Async command implementation following MAUI best practices.
/// Supports execution tracking and proper error handling.
/// </summary>
public sealed class AsyncCommand : ICommand, INotifyPropertyChanged
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets whether the command is currently executing.
    /// </summary>
    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            if (_isExecuting != value)
            {
                _isExecuting = value;
                try
                {
                    if (MainThread.IsMainThread)
                    {
                        PropertyChanged?.Invoke(
                            this,
                            new PropertyChangedEventArgs(nameof(IsExecuting))
                        );
                        RaiseCanExecuteChanged();
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            PropertyChanged?.Invoke(
                                this,
                                new PropertyChangedEventArgs(nameof(IsExecuting))
                            );
                            RaiseCanExecuteChanged();
                        });
                    }
                }
                catch { }
            }
        }
    }

    public bool CanExecute(object? parameter)
    {
        if (IsExecuting)
            return false;

        return _canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter) => _ = ExecuteAsync();

    public async Task ExecuteAsync()
    {
        if (!CanExecute(null))
            return;

        IsExecuting = true;

        try
        {
            await _execute().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AsyncCommand exception: {ex}");
        }
        finally
        {
            IsExecuting = false;
        }
    }

    public void RaiseCanExecuteChanged()
    {
        if (MainThread.IsMainThread)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
