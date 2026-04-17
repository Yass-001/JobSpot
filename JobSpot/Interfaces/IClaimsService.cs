using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JobSpot.Interfaces
{
    public interface IClaimsService
    {
        Task<bool> HasClaimAsync(IdentityUser user, string claimType, string claimValue);
        Task<bool> HasPermissionAsync(IdentityUser user, string permission);
        Task<List<Claim>> GetUserClaimsAsync(IdentityUser user);
        Task<List<Claim>> GetRoleClaimsAsync(string roleName);
        Task AddClaimToUserAsync(IdentityUser user, string claimType, string claimValue);
        Task RemoveClaimFromUserAsync(IdentityUser user, string claimType, string claimValue);
        Task<string?> GetUserTokenAsync(IdentityUser user, string loginProvider, string tokenName);
        Task SetUserTokenAsync(IdentityUser user, string loginProvider, string tokenName, string? tokenValue);

    }
}
