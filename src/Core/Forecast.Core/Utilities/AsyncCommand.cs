using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;

namespace Forecast.Utilities;

public sealed class AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    : ICommand,
        INotifyPropertyChanged
{
    private readonly Func<Task> _execute = execute;
    private readonly Func<bool>? _canExecute = canExecute;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

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
            await _execute();
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
