using hmt_energy_csharp.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace hmt_energy_csharp.Web.Pages;

public abstract class hmt_energy_csharpPageModel : AbpPageModel
{
    protected hmt_energy_csharpPageModel()
    {
        LocalizationResourceType = typeof(hmt_energy_csharpResource);
    }
}