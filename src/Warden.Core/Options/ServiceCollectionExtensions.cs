using Microsoft.Extensions.DependencyInjection;

namespace Warden.Core.Options;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMutableOptions(
        this IServiceCollection services,
        string filePath
    )
    {
        services.Configure<MutableOptionsWrapper>(options => options.FilePath = filePath);
        services.AddSingleton(
            typeof(IOptionsMutableStore<>),
            typeof(OptionsMutableJsonFileStore<>)
        );
        services.AddScoped(typeof(IOptionsMutable<>), typeof(OptionsMutable<>));
        return services;
    }
}
