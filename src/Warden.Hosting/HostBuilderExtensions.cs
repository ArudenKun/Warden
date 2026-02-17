using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Warden.Hosting.Internals;

namespace Warden.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureAvaloniaHosting<TApplication>(
        this IHostBuilder builder,
        Action<AppBuilder> configure
    )
        where TApplication : Application =>
        builder.ConfigureAvaloniaHosting<TApplication>((_, appBuilder) => configure(appBuilder));

    public static IHostBuilder ConfigureAvaloniaHosting<TApplication>(
        this IHostBuilder builder,
        Action<IServiceProvider, AppBuilder> configure
    )
        where TApplication : Application =>
        builder.ConfigureServices(services =>
            services
                .AddSingleton<TApplication>()
                .AddSingleton<Application>(sp => sp.GetRequiredService<TApplication>())
                .AddSingleton(sp =>
                {
                    var appBuilder = AppBuilder.Configure(sp.GetRequiredService<TApplication>);
                    configure.Invoke(sp, appBuilder);
                    return appBuilder;
                })
                .AddSingleton<AvaloniaThread>()
                .AddHostedService<AvaloniaHostedService>()
        );
}
