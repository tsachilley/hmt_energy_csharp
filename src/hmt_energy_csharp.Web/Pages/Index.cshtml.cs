using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Web.Pages;

public class IndexModel : hmt_energy_csharpPageModel
{
    public void OnGet()
    {
    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}