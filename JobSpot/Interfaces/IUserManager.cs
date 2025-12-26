using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JobSpot.Interfaces
{
    public interface IUserManager
    {
        string GetUserId(ClaimsPrincipal user);
        Task<IdentityUser> GetUserAsync(ClaimsPrincipal user);

        // Add other necessary user management methods as needed

        // Checks if a user is in a specific role
        //Task<bool> IsInRoleAsync(IdentityUser user, string role);

        // Finds a user by their ID
        //Task<IdentityUser> FindByIdAsync(string userId);

        // Finds a user by their username/email
        //Task<IdentityUser> FindByNameAsync(string userName);
    }
}
