using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R3;
using R3.ObservableEvents;
using Serilog.Core;
using Volo.Abp;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Warden.Services.Settings;
using Warden.Settings;
using Warden.Utilities;

namespace Warden;

[DependsOn(typeof(AbpBackgroundJobsHangfireModule))]
public sealed class WardenModule : AbpModule
{
    private IDisposable? _subscriptions;

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddObjectAccessor<LoggingLevelSwitch>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddServices();
        context.Services.AddSingleton(sp =>
            sp.GetRequiredService<IObjectAccessor<LoggingLevelSwitch>>().Value
            ?? throw new InvalidOperationException("LoggingLevelSwitch is not set")
        );
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var settingsService = context.ServiceProvider.GetRequiredService<ISettingsService>();
        var loggingSetting = settingsService.Get<LoggingSetting>();
        context.ServiceProvider.GetRequiredService<ObjectAccessor<LoggingLevelSwitch>>().Value =
            LogHelper.LoggingLevelSwitch;
        LogHelper.Initialize(loggingSetting);

        _subscriptions = Disposable.Combine(
            AppDomain
                .CurrentDomain.Events()
                .UnhandledException.Subscribe(e =>
                    HandleUnhandledException(
                        (Exception)e.ExceptionObject,
                        AppHelper.Name,
                        context.ServiceProvider
                    )
                ),
            RxEvents.TaskSchedulerUnobservedTaskException.Subscribe(e =>
            {
                HandleUnhandledException(
                    e.Exception,
                    $"{AppHelper.Name} Task",
                    context.ServiceProvider
                );
                e.SetObserved();
            }),
            Dispatcher
                .UIThread.Events()
                .UnhandledException.Subscribe(e =>
                {
                    HandleUnhandledException(
                        e.Exception,
                        $"{AppHelper.Name} UI",
                        context.ServiceProvider
                    );
                    e.Handled = true;
                })
        );
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<WardenModule>();
        logger.LogInformation("Shutting Down Warden");
        _subscriptions?.Dispose();
        LogHelper.Cleanup();
    }

    private void HandleUnhandledException(
        Exception exception,
        string category,
        IServiceProvider serviceProvider
    )
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(category);
        logger.LogError(exception, "Unhandled Exception");
        // DispatchHelper.Invoke(() =>
        //     _toastService.ShowExceptionToast(exception, $"{category} Exception")
        // );
    }
}
