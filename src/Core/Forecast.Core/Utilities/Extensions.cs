using Microsoft.Maui.ApplicationModel;

namespace Forecast.Utilities;

public static class Extensions
{
    public static void RunOnMainThread(this Action action)
    {
        if (action is null)
            return;

        if (MainThread.IsMainThread)
            action();
        else
            MainThread.BeginInvokeOnMainThread(action);
    }

    public static Task RunOnMainThreadAsync(this Func<Task> func)
    {
        if (func is null)
            return Task.CompletedTask;

        if (MainThread.IsMainThread)
            return func();

        return MainThread.InvokeOnMainThreadAsync(func);
    }
}
