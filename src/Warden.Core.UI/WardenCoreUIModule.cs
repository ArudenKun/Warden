using Autofac;
using Autofac.Core.Resolving.Pipeline;
using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using R3;
using R3.ObservableEvents;
using Volo.Abp;
using Volo.Abp.Modularity;
using Warden.Dependency;

namespace Warden.Core;

// ReSharper disable once InconsistentNaming
[DependsOn(typeof(WardenCoreModule))]
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

        context.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
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

                            var instance = context.Instance;

                            if (instance is ViewModelBase viewModelBase)
                                context.Resolve<IMessenger>().RegisterAll(viewModelBase);

                            if (!context.NewInstanceActivated)
                                return;

                            switch (instance)
                            {
                                case IInitializer initializer:
                                    initializer.Initialize();
                                    break;
                                case Application application:
                                    application.DataTemplates.AddIfNotContains(
                                        context.Resolve<ViewLocator>()
                                    );
                                    break;
                            }
                        }
                    )
            );
}
