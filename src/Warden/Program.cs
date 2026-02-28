using Avalonia;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Velopack;
using Volo.Abp;
using Volo.Abp.IO;
using Volo.Abp.Modularity.PlugIns;
using Warden.Core.Extensions;
using Warden.Services.Settings;
using Warden.Settings;
using Warden.Utilities;

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
        LogHelper.LoggingLevelSwitch = new LoggingLevelSwitch(loggingSetting.LogEventLevel);
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

        var abpApplication = AbpApplicationFactory.Create<WardenModule>(options =>
        {
            var pluginDir = AppHelper.DataDir.CombinePath("Plugins");
            DirectoryHelper.CreateIfNotExists(pluginDir);
            options.PlugInSources.AddFolder(pluginDir);
            options.Services.AddLogging(builder =>
                builder.ClearProviders().AddSerilog(dispose: true)
            );
            options.UseAutofac();
        });

        try
        {
            VelopackApp.Build().SetLogger(new VelopackLogger()).Run();
            abpApplication.Initialize();

            BuildAvaloniaApp(abpApplication.ServiceProvider).StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            // var loggerFactory = abpApplication.ServiceProvider.GetRequiredService<ILoggerFactory>();
            // var logger = loggerFactory.CreateLogger(AppConsts.Name);
            // logger.LogError(e, "Unhandled Exception");
            throw;
        }
        finally
        {
            abpApplication.Shutdown();
            abpApplication.Dispose();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once UnusedMember.Global
    public static AppBuilder BuildAvaloniaApp() =>
        BuildAvaloniaApp(new ServiceCollection().BuildServiceProvider());

    private static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider) =>
        AppBuilder
            .Configure(serviceProvider.GetRequiredService<App>)
            .UseR3(ex => LogHelper.Error(ex, "Unhandled R3 Exception"))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
