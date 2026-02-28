using Autofac;
using Autofac.Core.Resolving.Pipeline;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R3;
using R3.ObservableEvents;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;
using Volo.Abp.Timing;
using Volo.Abp.VirtualFileSystem;
using Warden.Data;
using Warden.Dependency;
using Warden.Localization;
using Warden.Utilities;
using Warden.ViewModels;

namespace Warden;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpBackgroundWorkersQuartzModule),
    typeof(AbpBackgroundJobsQuartzModule),
    typeof(AbpGuidsModule)
)]
public sealed class WardenModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddConventionalRegistrar(new ViewModelConventionalRegistrar());
        context.Services.AddObjectAccessor<TopLevel>();

        PreConfigure<AbpQuartzOptions>(options =>
            options.Configurator = builder =>
            {
                builder.UseSimpleTypeLoader();
                // builder.UsePersistentStore(storeOptions =>
                // {
                //     storeOptions.UseSystemTextJsonSerializer();
                //     storeOptions.UseMicrosoftSQLite(
                //         AppHelper.BackgroundJobsWorkersConnectionString
                //     );
                // });
            }
        );
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ILocalizer, AbpLocalizer<WardenResource>>();
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources.Add<WardenResource>("en").AddVirtualJson("/Localization/Warden");
            options.DefaultResourceType = typeof(WardenResource);
        });

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });

        context.Services.AddAbpDbContext<WardenDbContext>(options =>
            options.AddDefaultRepositories(true)
        );

        ConfigureVirtualFileSystem(context.Services.GetAbpHostEnvironment());
        ConfigureInitializer(context.Services.GetContainerBuilder());

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
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(AppConsts.Name);
        logger.LogInformation("Shutting Down");
        LogHelper.Cleanup();
    }

    private void ConfigureVirtualFileSystem(IAbpHostEnvironment abpHostEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<WardenModule>();

            if (abpHostEnvironment.IsDevelopment())
            {
                // options.FileSets.ReplaceEmbeddedByPhysical<ZiraDomainSharedModule>(
                //     Path.Combine(
                //         hostEnvironment.ContentRootPath,
                //         string.Format("..{0}Zira.Domain.Shared", Path.DirectorySeparatorChar)
                //     )
                // );
                options.FileSets.ReplaceEmbeddedByPhysical<WardenModule>(AppHelper.ContentRootDir);
            }
        });
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

                            if (context.Instance is IInitializer initializer)
                                initializer.Initialize();
                        }
                    )
            );
}
