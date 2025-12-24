using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JobSpot.Interfaces
{
    public interface IUserManager
    {
        string GetUserId(ClaimsPrincipal user);
        Task<IdentityUser> GetUserAsync(ClaimsPrincipal user);

        // Add other necessary user management methods as needed
    }
}
