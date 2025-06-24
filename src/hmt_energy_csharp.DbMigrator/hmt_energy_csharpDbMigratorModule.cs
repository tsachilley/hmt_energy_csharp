using hmt_energy_csharp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace hmt_energy_csharp.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(hmt_energy_csharpEntityFrameworkCoreModule),
    typeof(hmt_energy_csharpApplicationContractsModule)
    )]
public class hmt_energy_csharpDbMigratorModule : AbpModule
{
}