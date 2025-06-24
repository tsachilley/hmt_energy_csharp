using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace hmt_energy_csharp.Web.Controllers
{
    public class HomeController : AbpController
    {
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}