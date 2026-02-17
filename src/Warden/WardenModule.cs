using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceScan.SourceGenerator;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Warden.Services.Settings;
using Warden.Settings;
using Warden.Utilities;
using Warden.ViewModels;

namespace Warden;

[DependsOn(typeof(AbpAutofacModule))]
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

        var containerBuilder = context.Services.GetObject<ContainerBuilder>();
        ConfigureMessenger(containerBuilder);
    }

    public override void PostConfigureServices(ServiceConfigurationContext context) { }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var settingsService = context.ServiceProvider.GetRequiredService<ISettingsService>();
        var loggingSetting = settingsService.Get<LoggingSetting>();
        LogHelper.Initialize(loggingSetting);
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(AppHelper.Name);
        logger.LogInformation("Shutting Down");
        LogHelper.Cleanup();
    }

    private void ConfigureMessenger(ContainerBuilder builder)
    {
        builder.ComponentRegistryBuilder.Registered += ComponentRegistryBuilderOnRegistered;
    }

    private void ComponentRegistryBuilderOnRegistered(
        object? sender,
        ComponentRegisteredEventArgs e
    )
    {
        e.ComponentRegistration.PipelineBuilding += ComponentRegistrationOnPipelineBuilding;
    }

    private void ComponentRegistrationOnPipelineBuilding(object? sender, IResolvePipelineBuilder e)
    {
        e.Use(
            PipelinePhase.Activation,
            (ctx, next) =>
            {
                next(ctx);
                if (!ctx.NewInstanceActivated || ctx.Instance is null)
                    return;

                ConfigureMessenger(ctx.Instance);
            }
        );
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IRecipient<>),
        CustomHandler = nameof(ConfigureMessengerHandler)
    )]
    private partial void ConfigureMessenger(object instance);

    private void ConfigureMessengerHandler<T, TMessage>(object instance)
        where T : class, IRecipient<TMessage>
        where TMessage : class
    {
        if (instance is T recipient)
        {
            WeakReferenceMessenger.Default.Register(recipient);
        }
    }
}
