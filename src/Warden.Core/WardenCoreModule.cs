using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;

namespace Warden.Core;

[DependsOn(
    typeof(AbpCachingModule),
    typeof(AbpObjectExtendingModule),
    typeof(AbpObjectMappingModule),
    typeof(AbpTimingModule)
)]
public sealed class WardenCoreModule : AbpModule { }
