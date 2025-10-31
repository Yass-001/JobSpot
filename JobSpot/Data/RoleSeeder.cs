using JobSpot.Constants;
using Microsoft.AspNetCore.Identity;

namespace JobSpot.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            // can be made an array/list and looped through if more roles are added in future

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (! await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.Employer))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Employer));
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.JobSeeker))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.JobSeeker));
            }
        }
    }
}
