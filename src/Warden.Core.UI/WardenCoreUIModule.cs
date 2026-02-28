using Autofac;
using Autofac.Core.Resolving.Pipeline;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using R3;
using R3.ObservableEvents;
using Volo.Abp;
using Volo.Abp.Modularity;
using Warden.Dependency;

namespace Warden.Core;

// ReSharper disable once InconsistentNaming
public sealed class WardenCoreUIModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddConventionalRegistrar(new ViewConventionalRegistrar());
        context.Services.AddConventionalRegistrar(new ViewModelConventionalRegistrar());
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var containerBuilder = context.Services.GetObjectOrNull<ContainerBuilder>();
        if (containerBuilder is not null)
            ConfigureInitializer(context.Services.GetContainerBuilder());
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
#if DEBUG
        Ioc.Default.ConfigureServices(context.ServiceProvider);
#endif
    }

    private static void ConfigureInitializer(ContainerBuilder containerBuilder) =>
        containerBuilder
            .ComponentRegistryBuilder.Events()
            .Registered.Subscribe(args =>
                args.ComponentRegistration.PipelineBuilding += (_, builder) =>
                    builder.Use(
                        PipelinePhase.Activation,
                        (context, next) =>
                        {
                            next(context);
                            if (!context.NewInstanceActivated)
                                return;

                            var instance = context.Instance!;
                            if (instance is IInitializer initializer)
                                initializer.Initialize();
                        }
                    )
            );
}
