using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace JobSpot.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class GoogleModel : PageModel
    {
        public async Task<IActionResult> OnGet(string returnUrl)
        {
            //await HttpContext.ChallengeAsync("Google");

            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new InvalidOperationException("Invalid return URL."); // don`t do for production, just for testing?!
            }

            var properties = new AuthenticationProperties 
            { 
                RedirectUri = returnUrl 
            };

            return Challenge(properties, "Google"); 
        }
    }
}
