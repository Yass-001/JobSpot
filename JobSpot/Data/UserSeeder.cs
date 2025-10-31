using Microsoft.AspNetCore.Identity;
using JobSpot.Constants;

namespace JobSpot.Data
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Seed an admin user

            if (await userManager.FindByEmailAsync("admin@jobspot.com") == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin@jobspot.com",
                    Email = "admin@jobspot.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                }
            }
        }
    }
}
