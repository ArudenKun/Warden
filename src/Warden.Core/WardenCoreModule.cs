using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;

namespace Warden.Core;

[DependsOn(
    typeof(AbpCachingModule),
    typeof(AbpObjectExtendingModule),
    typeof(AbpObjectMappingModule),
    typeof(AbpBackgroundJobsModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpTimingModule),
    typeof(AbpGuidsModule)
)]
public sealed class WardenCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context) { }

    public override void OnApplicationShutdown(ApplicationShutdownContext context) { }
}
