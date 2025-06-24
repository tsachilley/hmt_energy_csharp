using hmt_energy_csharp.Localization;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp;

/* Inherit your application services from this class.
 */

public abstract class hmt_energy_csharpAppService : ApplicationService
{
    protected hmt_energy_csharpAppService()
    {
        LocalizationResource = typeof(hmt_energy_csharpResource);
    }
}