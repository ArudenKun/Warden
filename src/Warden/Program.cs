using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nito.Disposables;
using R3;
using R3.ObservableEvents;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Velopack;
using Volo.Abp;
using Volo.Abp.IO;
using Volo.Abp.Modularity.PlugIns;
using Warden.Core;
using Warden.Core.Extensions;
using Warden.Core.Options;
using Warden.Core.Settings;
using Warden.Options;
using Warden.Services;
using Warden.Utilities;
using Warden.ViewModels;
using Disposable = R3.Disposable;

namespace Warden;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        LogHelper.LoggingLevelSwitch = new LoggingLevelSwitch(
            new SettingsService(
                new OptionsWrapper<SettingsServiceOptions>(
                    new SettingsServiceOptions { FilePath = AppHelper.SettingsPath }
                ),
                NullLogger<SettingsService>.Instance
            )
                .Get<LoggingOptions>()
                .LogEventLevel
        );
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LogHelper.LoggingLevelSwitch)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithDemystifiedStackTraces()
            .WriteTo.Async(c =>
                c.File(
                    AppHelper.LogsDir.CombinePath("log.txt"),
                    outputTemplate: LoggingOptions.Template,
                    retainedFileTimeLimit: 30.Days(),
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    shared: true
                )
            )
            .WriteTo.Async(c => c.Console(outputTemplate: LoggingOptions.Template))
            .CreateLogger();

        var abpApplication = await AbpApplicationFactory.CreateAsync<WardenModule>(options =>
        {
            var pluginDir = AppHelper.DataDir.CombinePath("Plugins");
            DirectoryHelper.CreateIfNotExists(pluginDir);
            options.PlugInSources.AddFolder(pluginDir);
            options.Services.AddLogging(builder =>
                builder.ClearProviders().AddSerilog(dispose: true)
            );
            options.Services.AddMutableOptions(AppHelper.DataDir.CombinePath("test-options.json"));
            options.UseAutofac();
        });

        try
        {
            VelopackApp.Build().SetLogger(new VelopackLogger()).Run();
            await abpApplication.InitializeAsync();
            var builder = BuildAvaloniaApp();
            builder.AfterSetup(_ =>
            {
                LogHelper.Initialize(
                    abpApplication
                        .ServiceProvider.GetRequiredService<ISettingsService>()
                        .Get<LoggingOptions>()
                );
                abpApplication.ServiceProvider.GetRequiredService<IThemeService>().Initialize();

                var subscription = SubscribeExceptionEvents(
                    abpApplication.ServiceProvider.GetRequiredService<ILoggerFactory>(),
                    abpApplication.ServiceProvider.GetRequiredService<IToastService>()
                );

                if (
                    builder.Instance
                    is not {
                        ApplicationLifetime: IClassicDesktopStyleApplicationLifetime desktop
                    } instance
                )
                    throw new InvalidOperationException("Application not yet initialized");

                instance.DataTemplates.AddIfNotContains(
                    abpApplication.ServiceProvider.GetRequiredService<ViewLocator>()
                );

                desktop.Exit += (_, _) => subscription.Dispose();
                desktop.MainWindow =
                    instance
                        .DataTemplates[0]
                        .Build(
                            abpApplication.ServiceProvider.GetRequiredService<MainWindowViewModel>()
                        ) as Window;
            });
            return builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogHelper.Error(e, "Unhandled Exception");
            throw;
        }
        finally
        {
            await abpApplication.ShutdownAsync();
            await abpApplication.ToAsyncDisposable().DisposeAsync();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once UnusedMember.Global
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UseR3(ex => LogHelper.Error(ex, "Unhandled R3 Exception"))
            .UsePlatformDetect()
            .LogToTrace();

    private static IDisposable SubscribeExceptionEvents(
        ILoggerFactory loggerFactory,
        IToastService toastService
    ) =>
        Disposable.Combine(
            AppDomain
                .CurrentDomain.Events()
                .UnhandledException.Subscribe(e =>
                    HandleUnhandledException(
                        (Exception)e.ExceptionObject,
                        AppConsts.Name,
                        loggerFactory,
                        toastService
                    )
                ),
            RxEvents.TaskSchedulerUnobservedTaskException.Subscribe(e =>
            {
                HandleUnhandledException(e.Exception, "Task", loggerFactory, toastService);
                e.SetObserved();
            }),
            Dispatcher
                .UIThread.Events()
                .UnhandledException.Subscribe(e =>
                {
                    HandleUnhandledException(e.Exception, "UI", loggerFactory, toastService);
                    e.Handled = true;
                })
        );

    private static void HandleUnhandledException(
        Exception exception,
        string category,
        ILoggerFactory loggerFactory,
        IToastService toastService
    )
    {
        var logger = loggerFactory.CreateLogger(category);
        logger.LogError(exception, "Unhandled Exception");

        var content = exception is UserFriendlyException userFriendly
            ? !userFriendly.Details.IsNullOrWhiteSpace()
                ? $"{userFriendly.Message}\n{userFriendly.Details}"
                : userFriendly.Message
            : exception.Message;

        DispatchHelper.Invoke(() =>
            toastService.ShowExceptionToast($"{category} Exception", content)
        );
    }
}
