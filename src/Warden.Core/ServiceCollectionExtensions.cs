using Microsoft.Extensions.DependencyInjection;
using Warden.Core.Playit;

namespace Warden.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSingleton<IPlayitManager, PlayitManager>();
        return services;
    }
}
