using JobSpot.Constants;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JobSpot.Data
{
    public class ClaimSeeder
    {
        public static async Task SeedClaimsAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); // ? IUserManager is not registered, so we use UserManager<IdentityUser> directly

            // Seed role claims
            await SeedRoleClaimsAsync(roleManager);

            // Seed user claims
            await SeedUserClaimsAsync(userManager);
        }

        private static async Task SeedRoleClaimsAsync(RoleManager<IdentityRole> roleManager)
        {
            // Admin role claims
            var adminRole = await roleManager.FindByNameAsync(UserRoles.Admin);
            if (adminRole != null)
            {
                var adminClaims = new[]
                {
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.CreateJobPosting),
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.EditJobPosting),
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.DeleteJobPosting),
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.ViewJobPosting),
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.ManageUsers)
                };

                foreach (var claim in adminClaims)
                {
                    var exists = await roleManager.GetClaimsAsync(adminRole);
                    if (!exists.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                    {
                        await roleManager.AddClaimAsync(adminRole, claim);
                    }
                }
            }

            // Employer role claims
            var employerRole = await roleManager.FindByNameAsync(UserRoles.Employer);
            if (employerRole != null)
            {
                var employerClaims = new[]
                {
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.CreateJobPosting),
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.EditJobPosting),
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.DeleteJobPosting),
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.ViewJobPosting)
                };

                foreach (var claim in employerClaims)
                {
                    var exists = await roleManager.GetClaimsAsync(employerRole);
                    if (!exists.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                    {
                        await roleManager.AddClaimAsync(employerRole, claim);
                    }
                }
            }

            // JobSeeker role claims
            var jobSeekerRole = await roleManager.FindByNameAsync(UserRoles.JobSeeker);
            if (jobSeekerRole != null)
            {
                var jobSeekerClaims = new[]
                {
                    new Claim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.ViewJobPosting)
                };

                foreach (var claim in jobSeekerClaims)
                {
                    var exists = await roleManager.GetClaimsAsync(jobSeekerRole);
                    if (!exists.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                    {
                        await roleManager.AddClaimAsync(jobSeekerRole, claim);
                    }
                }
            }
        }

        private static async Task SeedUserClaimsAsync(UserManager<IdentityUser> userManager)
        {
            // Add custom claims to specific users
            var adminUser = await userManager.FindByEmailAsync("admin@jobspot.com");
            if (adminUser != null)
            {
                var adminUserClaims = new[]
                {
                    new Claim(ApplicationClaimTypes.Department, "Management"),
                    new Claim(ApplicationClaimTypes.Level, "Senior")
                };

                var existingClaims = await userManager.GetClaimsAsync(adminUser);
                foreach (var claim in adminUserClaims)
                {
                    if (!existingClaims.Any(c => c.Type == claim.Type))
                    {
                        await userManager.AddClaimAsync(adminUser, claim);
                    }
                }
            }

            var employerUser = await userManager.FindByEmailAsync("employer@jobspot.com");
            if (employerUser != null)
            {
                var employerUserClaims = new[]
                {
                    new Claim(ApplicationClaimTypes.Department, "HR"),
                    new Claim(ApplicationClaimTypes.Level, "Manager")
                };

                var existingClaims = await userManager.GetClaimsAsync(employerUser);
                foreach (var claim in employerUserClaims)
                {
                    if (!existingClaims.Any(c => c.Type == claim.Type))
                    {
                        await userManager.AddClaimAsync(employerUser, claim);
                    }
                }
            }
        }
    }
}

