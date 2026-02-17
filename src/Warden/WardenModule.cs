using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceScan.SourceGenerator;
using StatePulse.Net;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;
using Warden.Utilities;
using Warden.ViewModels;

namespace Warden;

[DependsOn(typeof(AbpAutofacModule), typeof(AbpQuartzModule))]
public sealed partial class WardenModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddConventionalRegistrar(new ViewModelConventionalRegistrar());
        context.Services.AddObjectAccessor<TopLevel>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton(sp =>
            sp.GetRequiredService<IObjectAccessor<TopLevel>>().Value
            ?? throw new InvalidOperationException("TopLevel is not yet set")
        );
        context.Services.AddTransient(sp => sp.GetRequiredService<TopLevel>().Clipboard!);
        context.Services.AddTransient(sp => sp.GetRequiredService<TopLevel>().StorageProvider);
        context.Services.AddTransient(sp => sp.GetRequiredService<TopLevel>().Launcher);
        context.Services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        context.Services.AddSingleton<ISukiToastManager, SukiToastManager>();
        context.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        context.Services.AddStatePulseServices();
        ConfigureStatePulse(context.Services);
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(AppHelper.Name);
        logger.LogInformation("Shutting Down");
        LogHelper.Cleanup();
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IAction),
        TypeNameFilter = "*Action",
        CustomHandler = nameof(ConfigureStatePulseHandler)
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IStateFeature),
        TypeNameFilter = "*State",
        CustomHandler = nameof(ConfigureStatePulseHandler)
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IEffect<>),
        TypeNameFilter = "*Effect",
        CustomHandler = nameof(ConfigureStatePulseHandler)
    )]
    private partial void ConfigureStatePulse(IServiceCollection services);

    private static void ConfigureStatePulseHandler<T>(IServiceCollection services)
    {
        services.AddStatePulseService<T>();
    }
}
