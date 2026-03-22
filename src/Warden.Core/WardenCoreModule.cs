using Volo.Abp.Caching;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Warden.Core.Modrinth;

namespace Warden.Core;

[DependsOn(
    typeof(AbpCachingModule),
    typeof(AbpObjectExtendingModule),
    typeof(AbpObjectMappingModule),
    typeof(AbpTimingModule),
    typeof(AbpGuidsModule)
)]
public sealed class WardenCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpClockOptions>(options => options.Kind = DateTimeKind.Utc);
        context.Services.AddRefitClients();
    }
}
