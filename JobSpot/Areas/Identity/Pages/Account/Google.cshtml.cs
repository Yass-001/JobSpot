using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobSpot.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class GoogleModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
