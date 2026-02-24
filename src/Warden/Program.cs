using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Velopack;
using Volo.Abp;
using Volo.Abp.IO;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;
using Warden.Core.Extensions;
using Warden.Hosting;
using Warden.Services.Settings;
using Warden.Settings;
using Warden.Utilities;

namespace Warden;

public static class Program
{
    private static bool _isInitialized;

    private static IHost Host { get; } =
        new HostBuilder()
            .ConfigureDefaults(null)
            .ConfigureAbpApplication<WardenModule>(
                (ctx, options) =>
                {
                    options.Services.AddObjectAccessor(ctx.HostingEnvironment);
                    var pluginDir = AppHelper.DataDir.CombinePath("Plugins");
                    DirectoryHelper.CreateIfNotExists(pluginDir);
                    options.PlugInSources.AddFolder(pluginDir);
                }
            )
            .ConfigureAvaloniaHosting<App>(appBuilder =>
                appBuilder
                    .UseR3(ex => LogHelper.Error(ex, "Unhandled R3 Exception"))
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace()
            )
            .UseAutofac()
            .UseConsoleLifetime()
            .UseSerilog(dispose: true)
            .Build();

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main()
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

        try
        {
            VelopackApp.Build().SetLogger(new VelopackLogger()).Run();
            if (!_isInitialized)
            {
                Host.Initialize();
                _isInitialized = true;
            }
#if DEBUG
            Ioc.Default.ConfigureServices(Host.Services);
#endif
            Host.Run();
        }
        catch (Exception e)
        {
            var loggerFactory = Host.Services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(AppConsts.Name);
            // logger.LogException(e);
            logger.LogError(e, "Unhandled Exception");
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once UnusedMember.Global
    public static AppBuilder BuildAvaloniaApp()
    {
        if (!_isInitialized)
        {
            Host.Initialize();
            _isInitialized = true;
        }
#if DEBUG
        Ioc.Default.ConfigureServices(Host.Services);
#endif
        return Host.Services.GetRequiredService<AppBuilder>();
    }

    private static void Initialize(this IHost host)
    {
        var application =
            host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>();
        var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStopping.Register(() => application.ShutdownAsync());
        applicationLifetime.ApplicationStopped.Register(() => application.Dispose());
        application.Initialize(host.Services);
    }

    private static IHostBuilder ConfigureAbpApplication<TStartupModule>(
        this IHostBuilder builder,
        Action<HostBuilderContext, AbpApplicationCreationOptions>? configure = null
    )
        where TStartupModule : IAbpModule =>
        builder.ConfigureServices(
            (ctx, services) =>
                services.AddApplication<TStartupModule>(options =>
                {
                    options.Services.ReplaceConfiguration(ctx.Configuration);
                    configure?.Invoke(ctx, options);
                    if (options.Environment.IsNullOrWhiteSpace())
                    {
                        options.Environment = ctx.HostingEnvironment.EnvironmentName;
                    }
                })
        );
}
