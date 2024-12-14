using System;
using System.Threading.Tasks;
using Serilog;

namespace NzbDrone.Common.Instrumentation;

public static class GlobalExceptionHandlers
{
    public static void Register()
    {
        AppDomain.CurrentDomain.UnhandledException += HandleAppDomainException;
        TaskScheduler.UnobservedTaskException += HandleTaskException;
    }

    private static void HandleTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var exception = e.Exception;

        if (exception.InnerException is ObjectDisposedException disposedException &&
            disposedException.ObjectName == "System.Net.HttpListenerRequest")
        {
            // We don't care about web connections
            return;
        }

        Log.Error(exception, "Task Error: {Message}", exception.Message);
        e.SetObserved();
    }

    private static void HandleAppDomainException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception exception)
        {
            return;
        }

        if (exception is NullReferenceException &&
            exception.ToString().Contains("Microsoft.AspNet.SignalR.Transports.TransportHeartbeat.ProcessServerCommand"))
        {
            Log.Warning("SignalR Heartbeat interrupted");
            return;
        }

        Log.Fatal(exception, "EPIC FAIL: {Message}", exception.Message);
    }
}
