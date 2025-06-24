using hmt_energy_csharp.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace hmt_energy_csharp.Controllers;

/* Inherit your controllers from this class.
 */

[RemoteService]
[ControllerName("Customer")]
[Route("api/customer")]
public abstract class hmt_energy_csharpController : AbpControllerBase
{
    protected hmt_energy_csharpController()
    {
        LocalizationResource = typeof(hmt_energy_csharpResource);
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetActionResult(int id)
    {
        return Content(id.ToString());
    }
}