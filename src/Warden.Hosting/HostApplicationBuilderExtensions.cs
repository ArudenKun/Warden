using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Warden.Hosting.Internals;

namespace Warden.Hosting;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddAvaloniaHosting<TStartupModule, TApplication>(
        this IHostApplicationBuilder builder,
        Action<AppBuilder> configure,
        Action<AbpApplicationCreationOptions>? configureAbpCreationOptions = null
    )
        where TStartupModule : AbpModule
        where TApplication : Application =>
        builder.AddAvaloniaHosting<TStartupModule, TApplication>(
            (_, appBuilder) => configure(appBuilder),
            configureAbpCreationOptions
        );

    public static IHostApplicationBuilder AddAvaloniaHosting<TStartupModule, TApplication>(
        this IHostApplicationBuilder builder,
        Action<IServiceProvider, AppBuilder> configure,
        Action<AbpApplicationCreationOptions>? configureAbpCreationOptions = null
    )
        where TStartupModule : AbpModule
        where TApplication : Application
    {
        builder.Services.AddApplication<TStartupModule>(options =>
        {
            options.Services.ReplaceConfiguration(builder.Configuration);
            options.Services.AddObjectAccessor<Window>();
            options.Services.AddObjectAccessor<TopLevel>();
            configureAbpCreationOptions?.Invoke(options);
            if (options.Environment.IsNullOrWhiteSpace())
                options.Environment = builder.Environment.EnvironmentName;
        });
        builder
            .Services.AddSingleton<TApplication>()
            .AddSingleton<Application>(sp => sp.GetRequiredService<TApplication>())
            .AddSingleton(sp =>
            {
                var appBuilder = AppBuilder.Configure(sp.GetRequiredService<TApplication>);
                configure.Invoke(sp, appBuilder);
                return appBuilder;
            })
            .AddSingleton<TopLevel>(sp =>
                sp.GetRequiredService<IObjectAccessor<TopLevel>>().Value
                ?? throw new InvalidOperationException("Avalonia is not yet initialized")
            )
            .AddSingleton<AvaloniaThread>()
            .AddHostedService<AvaloniaHostedService>();
        return builder;
    }

    public static IHostApplicationBuilder AddWindow<TWindow>(IHostApplicationBuilder builder)
        where TWindow : Window
    {
        return builder;
    }

    public static IHost InitializeAvaloniaHosting(this IHost host)
    {
        Initialize(host);
        if (
            Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop
        )
            host.Services.GetRequiredService<ObjectAccessor<TopLevel>>().Value = desktop.MainWindow;
        return host;
    }

    private static void Initialize(IHost host)
    {
        var application =
            host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>();
        var requiredService = host.Services.GetRequiredService<IHostApplicationLifetime>();
        var cancellationToken = requiredService.ApplicationStopping;
        cancellationToken.Register(() => application.Shutdown());
        cancellationToken = requiredService.ApplicationStopped;
        cancellationToken.Register(() => application.Dispose());
        application.Initialize(host.Services);
    }
}
