using System.Windows.Input;
using Forecast.Core.Models;

namespace Forecast.Utilities;

public sealed class AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    : ObservableObject,
        ICommand
{
    private readonly Func<Task> _execute = execute;
    private readonly Func<bool>? _canExecute = canExecute;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;
    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            if (_isExecuting != value)
            {
                _isExecuting = value;

                Extensions.RunOnMainThread(() =>
                {
                    OnPropertyChanged(nameof(IsExecuting));
                    RaiseCanExecuteChanged();
                });
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

        await _execute?.Invoke();

        IsExecuting = false;
    }

    public void RaiseCanExecuteChanged()
    {
        Extensions.RunOnMainThread(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }
}
