using hmt_energy_csharp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace hmt_energy_csharp;

[DependsOn(
    typeof(hmt_energy_csharpEntityFrameworkCoreTestModule)
    )]
public class hmt_energy_csharpDomainTestModule : AbpModule
{
}