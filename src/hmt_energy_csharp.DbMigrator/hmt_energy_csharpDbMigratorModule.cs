using hmt_energy_csharp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace hmt_energy_csharp.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(hmt_energy_csharpEntityFrameworkCoreModule),
    typeof(hmt_energy_csharpApplicationContractsModule)
    )]
public class hmt_energy_csharpDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "hmt_energy_csharp:"; });
    }
}