using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Warden.Core;
using Warden.Core.Modrinth;
using Warden.Core.Navigation.Extensions;
using Warden.Core.Settings;
using Warden.Data;
using Warden.Localization;
using Warden.Utilities;

namespace Warden;

[DependsOn(typeof(WardenCoreModule), typeof(WardenCoreUIModule))]
public sealed class WardenModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddObjectAccessor<TopLevel>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<ModrinthClientOptions>(options =>
            options.BaseUrl = ModrinthClientOptions.ProductionUrl
        );

        Configure<SettingsServiceOptions>(options =>
        {
            options.FilePath = AppHelper.SettingsPath;
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources.Add<WardenResource>("en").AddVirtualJson("/Localization/Warden");
            options.DefaultResourceType = typeof(WardenResource);
        });

        context.Services.AddAbpDbContext<WardenDbContext>(options =>
            options.AddDefaultRepositories(true)
        );

        ConfigureVirtualFileSystem(context.Services.GetAbpHostEnvironment());

        context.Services.AddNavigationHost();
        context.Services.AddSingleton<ILocalizer, AbpLocalizer<WardenResource>>();

        context.Services.AddSingleton(sp =>
            sp.GetRequiredService<IObjectAccessor<TopLevel>>().Value
            ?? throw new InvalidOperationException("TopLevel is not yet set")
        );
        context.Services.AddTransient(sp => sp.GetRequiredService<TopLevel>().Clipboard!);
        context.Services.AddTransient(sp => sp.GetRequiredService<TopLevel>().StorageProvider);
        context.Services.AddTransient(sp => sp.GetRequiredService<TopLevel>().Launcher);
        context.Services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        context.Services.AddSingleton<ISukiToastManager, SukiToastManager>();
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
}
