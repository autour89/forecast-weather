using Forecast.Core.Models;

namespace Forecast.Core.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    private bool _isRefreshing;
    private string _errorMessage = string.Empty;

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
