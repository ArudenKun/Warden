using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;
using Volo.Abp.Timing;
using Volo.Abp.VirtualFileSystem;
using Warden.Localization;
using Warden.Utilities;
using Warden.ViewModels;

namespace Warden;

[DependsOn(typeof(AbpAutofacModule), typeof(AbpQuartzModule), typeof(AbpGuidsModule))]
public sealed class WardenModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddConventionalRegistrar(new ViewModelConventionalRegistrar());
        context.Services.AddObjectAccessor<TopLevel>();
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

        ConfigureVirtualFileSystem(context.HostingEnvironment);

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

    private void ConfigureVirtualFileSystem(IHostEnvironment hostEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<WardenModule>();

            if (hostEnvironment.IsDevelopment())
            {
                // options.FileSets.ReplaceEmbeddedByPhysical<ZiraDomainSharedModule>(
                //     Path.Combine(
                //         hostEnvironment.ContentRootPath,
                //         string.Format("..{0}Zira.Domain.Shared", Path.DirectorySeparatorChar)
                //     )
                // );
                // options.FileSets.ReplaceEmbeddedByPhysical<ZiraDomainModule>(
                //     Path.Combine(
                //         hostEnvironment.ContentRootPath,
                //         string.Format("..{0}Zira.Domain", Path.DirectorySeparatorChar)
                //     )
                // );
                // options.FileSets.ReplaceEmbeddedByPhysical<ZiraApplicationContractsModule>(
                //     Path.Combine(
                //         hostEnvironment.ContentRootPath,
                //         string.Format(
                //             "..{0}Zira.Application.Contracts",
                //             Path.DirectorySeparatorChar
                //         )
                //     )
                // );
                // options.FileSets.ReplaceEmbeddedByPhysical<ZiraApplicationModule>(
                //     Path.Combine(
                //         hostEnvironment.ContentRootPath,
                //         string.Format("..{0}Zira.Application", Path.DirectorySeparatorChar)
                //     )
                // );
                // options.FileSets.ReplaceEmbeddedByPhysical<ZiraHttpApiModule>(
                //     Path.Combine(
                //         hostEnvironment.ContentRootPath,
                //         string.Format("..{0}..{0}src{0}Zira.HttpApi", Path.DirectorySeparatorChar)
                //     )
                // );
                options.FileSets.ReplaceEmbeddedByPhysical<WardenModule>(
                    hostEnvironment.ContentRootPath
                );
            }
        });
    }
}

file static class Extensions
{
    extension(ServiceConfigurationContext context)
    {
        public IHostEnvironment HostingEnvironment =>
            context.Services.GetObject<IHostEnvironment>();
    }
}
