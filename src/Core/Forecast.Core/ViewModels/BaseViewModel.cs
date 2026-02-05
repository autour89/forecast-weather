using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Forecast.Core.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private bool _isRefreshing;
    private string _errorMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public virtual async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(
        ref T storage,
        T value,
        [CallerMemberName] string? propertyName = null
    )
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public abstract class BaseViewModel<T> : BaseViewModel
{
    private T? _data;

    public T? Data
    {
        get => _data;
        set => SetProperty(ref _data, value);
    }
}
