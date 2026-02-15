using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Warden.Hosting.Internals;

/// <summary>
/// Manages the Avalonia thread and application lifecycle.
/// </summary>
internal sealed class AvaloniaThread
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly TaskCompletionSource _applicationExited = new();
    private readonly TaskCompletionSource _applicationStarted = new();
    private readonly Thread _uiThread;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaThread"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="applicationLifetime">The application lifetime.</param>
    public AvaloniaThread(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime
    )
    {
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
        _uiThread = new Thread(ThreadStart) { Name = "Avalonia Thread", IsBackground = true };
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _uiThread.SetApartmentState(ApartmentState.STA);
        }
    }

    /// <summary>
    /// Starts the Avalonia thread.
    /// The task completes when <see cref="IControlledApplicationLifetime.Startup"/> event fires.
    /// </summary>
    public Task StartAsync(CancellationToken token)
    {
        _uiThread.Start();
        return _applicationStarted.Task.WaitAsync(token);
    }

    /// <summary>
    /// Stops the Avalonia thread.
    /// The task completes when <see cref="IControlledApplicationLifetime.Exit"/> event fires.
    /// </summary>
    public Task StopAsync(CancellationToken token)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (
                Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime d
            )
            {
                d.TryShutdown();
            }
        });
        return _applicationExited.Task.WaitAsync(token);
    }

    /// <summary>
    /// The entry point for the Avalonia thread.
    /// </summary>
    private void ThreadStart()
    {
        var appBuilder = _serviceProvider.GetRequiredService<AppBuilder>();
        appBuilder.StartWithClassicDesktopLifetime(
            [],
            desktop =>
            {
                desktop.Startup += (_, _) => _applicationStarted.SetResult();
                desktop.Exit += (_, _) =>
                {
                    _applicationExited.TrySetResult();
                    _applicationLifetime.StopApplication();
                };
            }
        );
        // Avalonia stopped.
        _applicationExited.TrySetResult();
        _applicationLifetime.StopApplication();
    }
}
