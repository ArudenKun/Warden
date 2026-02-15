using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Velopack;
using Warden.Core.Extensions;
using Warden.Services.Settings;
using Warden.Settings;
using Warden.Utilities;
using Warden.Utilities.Extensions;

namespace Warden;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var settingService = new SettingsService();
        var loggingSetting = settingService.Get<LoggingSetting>();
        LogHelper.LoggingLevelSwitch = new LoggingLevelSwitch(
            loggingSetting.LogLevel.ToLogEventLevel()
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
                    outputTemplate: LoggingSetting.Template,
                    retainedFileTimeLimit: 30.Days(),
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    shared: true
                )
            )
            .WriteTo.Async(c => c.Console(outputTemplate: LoggingSetting.Template))
            .CreateLogger();

        try
        {
            VelopackApp.Build().SetLogger(new VelopackLogger()).Run();
            LogHelper.Information("Warden", "Starting");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogHelper.Error(e, "Unhandled Exception");
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once UnusedMember.Global
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UseR3(ex => LogHelper.Error(ex, "Unhandled R3 Exception"))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Avalonia
        services.AddSingleton<LoggingLevelSwitch>();
        services.AddTransient<IClipboard>(sp => sp.GetRequiredService<TopLevel>().Clipboard!);
        services.AddTransient<IStorageProvider>(sp =>
            sp.GetRequiredService<TopLevel>().StorageProvider
        );
        services.AddTransient<ILauncher>(sp => sp.GetRequiredService<TopLevel>().Launcher);
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.AddSingleton<ISukiToastManager, SukiToastManager>();

        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddSingleton<VelopackLogger>();

        services.AddSingleton<ISettingsService, SettingsService>();
        return services;
    }
}
