using JobSpot.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobSpot.Data
{
    public class AppDbContext : IdentityDbContext 
    {
        public DbSet<JobPosting> JobPostings { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            
        }
    }
}
