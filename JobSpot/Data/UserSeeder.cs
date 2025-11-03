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
            await CreateUserWithRole(userManager, "admin@jobspot.com", "Admin@123", UserRoles.Admin, "1234567890");
            await CreateUserWithRole(userManager, "jobseeker@jobspot.com", "Jobseeker@123", UserRoles.JobSeeker, "2345678901");
            await CreateUserWithRole(userManager, "employer@jobspot.com", "Employer@123", UserRoles.Employer, "3456789012");
        }

        private static async Task CreateUserWithRole(
            UserManager<IdentityUser> userManager, string email, string password, string role, string phoneNumber)
        {

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumber = phoneNumber,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Failed ro create user with email: {user.Email}. Errors: {String.Join(" ,", result.Errors)}");
                }
            }
        }
    }
}
