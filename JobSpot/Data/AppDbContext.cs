using JobSpot.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobSpot.Data
{
    // You cannot customize AspNetUserRoles without introducing your own user class.
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string,
    IdentityUserClaim<string>, ApplicationUserRole,
    IdentityUserLogin<string>, IdentityRoleClaim<string>,
    IdentityUserToken<string>>
    {
        public DbSet<JobPosting> JobPostings { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>()
                .Property(ur => ur.UserName)
                .HasMaxLength(256);
        }
    }
}
