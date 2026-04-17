using JobSpot.Constants;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JobSpot.Interfaces
{
    public class ClaimsService : IClaimsService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ClaimsService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> HasClaimAsync(IdentityUser user, string claimType, string claimValue)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            return claims.Any(c => c.Type == claimType && c.Value == claimValue);
        }

        public async Task<bool> HasPermissionAsync(IdentityUser user, string permission)
        {
            return await HasClaimAsync(user, ApplicationClaimTypes.Permission, permission);
        }

        public async Task<List<Claim>> GetUserClaimsAsync(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            return claims.ToList();
        }

        public async Task<List<Claim>> GetRoleClaimsAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return new List<Claim>();

            var claims = await _roleManager.GetClaimsAsync(role);
            return claims.ToList();
        }

        public async Task AddClaimToUserAsync(IdentityUser user, string claimType, string claimValue)
        {
            var claim = new Claim(claimType, claimValue);
            await _userManager.AddClaimAsync(user, claim);
        }

        public async Task RemoveClaimFromUserAsync(IdentityUser user, string claimType, string claimValue)
        {
            var claim = new Claim(claimType, claimValue);
            await _userManager.RemoveClaimAsync(user, claim);
        }

        public async Task<string?> GetUserTokenAsync(IdentityUser user, string loginProvider, string tokenName)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, loginProvider, tokenName);
        }

        public async Task SetUserTokenAsync(IdentityUser user, string loginProvider, string tokenName, string? tokenValue)
        {
            if (!string.IsNullOrEmpty(tokenValue))
            {
                await _userManager.SetAuthenticationTokenAsync(user, loginProvider, tokenName, tokenValue);
            }
        }
    }
}


