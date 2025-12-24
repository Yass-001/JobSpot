using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JobSpot.Interfaces
{
    public class UserManagerAdapter : IUserManager
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagerAdapter(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityUser> GetUserAsync(ClaimsPrincipal user)
        {
            return _userManager.GetUserAsync(user);
        }

        public string GetUserId(ClaimsPrincipal user)
        {
            return _userManager.GetUserId(user);
        }

        // Add other necessary user management methods as neededSystem.Security.Claims.
    }
}
