using Antelcat.AutoGen.ComponentModel.Diagnostic;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Warden.Core.Modrinth;

[AutoExtractInterface]
internal sealed class ModrinthClient : IModrinthClient, ITransientDependency
{
    private readonly ITransientCachedServiceProvider _cachedServiceProvider;

    public ModrinthClient(ITransientCachedServiceProvider cachedServiceProvider)
    {
        _cachedServiceProvider = cachedServiceProvider;
    }

    public INotifications Notifications => GetService<INotifications>();
    public IProjects Projects => GetService<IProjects>();
    public ITags Tags => GetService<ITags>();
    public ITeams Teams => GetService<ITeams>();
    public IThreads Threads => GetService<IThreads>();
    public IUsers Users => GetService<IUsers>();
    public IVersionFiles VersionFiles => GetService<IVersionFiles>();
    public IVersions Versions => GetService<IVersions>();

    private T GetService<T>()
        where T : notnull => _cachedServiceProvider.GetRequiredKeyedService<T>(typeof(T).FullName);
}
