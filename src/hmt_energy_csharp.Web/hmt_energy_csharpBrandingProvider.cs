using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace hmt_energy_csharp.Web;

[Dependency(ReplaceServices = true)]
public class hmt_energy_csharpBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "hmt_energy_csharp";
}