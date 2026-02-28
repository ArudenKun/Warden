using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Warden.Core.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsService(
        this IServiceCollection services,
        string filePath
    ) =>
        services.AddSingleton<ISettingsService>(sp =>
            (SettingsService)
                Activator.CreateInstance(
                    typeof(SettingsService),
                    filePath,
                    sp.GetService<JsonSerializerOptions>() ?? JsonSerializerOptions.Default
                )!
        );
}
