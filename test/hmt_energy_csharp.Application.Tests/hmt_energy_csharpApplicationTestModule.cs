using Volo.Abp.Modularity;

namespace hmt_energy_csharp;

[DependsOn(
    typeof(hmt_energy_csharpApplicationModule),
    typeof(hmt_energy_csharpDomainTestModule)
    )]
public class hmt_energy_csharpApplicationTestModule : AbpModule
{
}