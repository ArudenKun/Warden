using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Warden.Hosting.Internals;

namespace Warden.Hosting;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddAvaloniaHosting<TApplication>(
        this IHostApplicationBuilder builder,
        Action<AppBuilder> configure
    )
        where TApplication : Application =>
        builder.AddAvaloniaHosting<TApplication>((_, appBuilder) => configure(appBuilder));

    public static IHostApplicationBuilder AddAvaloniaHosting<TApplication>(
        this IHostApplicationBuilder builder,
        Action<IServiceProvider, AppBuilder> configure
    )
        where TApplication : Application
    {
        builder
            .Services.AddSingleton<TApplication>()
            .AddSingleton<Application>(sp => sp.GetRequiredService<TApplication>())
            .AddSingleton(sp =>
            {
                var appBuilder = AppBuilder.Configure(sp.GetRequiredService<TApplication>);
                configure.Invoke(sp, appBuilder);
                return appBuilder;
            })
            .AddSingleton<AvaloniaThread>()
            .AddHostedService<AvaloniaHostedService>();
        return builder;
    }
}
