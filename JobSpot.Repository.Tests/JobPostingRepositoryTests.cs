using JobSpot.Data;
using JobSpot.Models;
using JobSpot.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSpot.Repository.Tests
{
    public class JobPostingRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public JobPostingRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb-{Guid.NewGuid()}")
                .Options;
        }

        private AppDbContext CreateDbContext() => new AppDbContext(_dbContextOptions);

        [Fact]
        public async Task AddJobPosting_ShouldAddJobPostingToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new JobPostingRepository(context);
            var jobPosting = new JobPosting
            {
                Title = "Software Engineer",
                Description = "Develop and maintain software applications.",
                Company = "Tech Corp",
                Location = "New York, NY",
                PostedDate = DateTime.Now,
                IsApproved = true,
                UserId = "Test UserId"
            };

            // Act
            await repository.AddAsync(jobPosting);
            var addedJobPosting = await context.JobPostings.FirstOrDefaultAsync(jp => jp.Title == "Software Engineer");

            // Assert
            Assert.NotNull(addedJobPosting);
            Assert.Equal("Software Engineer", addedJobPosting.Title);
            Assert.Equal("Tech Corp", addedJobPosting.Company);
            Assert.Equal("New York, NY", addedJobPosting.Location);
            Assert.True(addedJobPosting.IsApproved);
            Assert.Equal("Develop and maintain software applications.", addedJobPosting.Description);
            Assert.Equal(jobPosting.PostedDate, addedJobPosting.PostedDate); // ?!
            Assert.Equal(jobPosting.Id, addedJobPosting.Id); // ?!
            Assert.Equal("Test UserId", addedJobPosting.UserId);
        }
    }
}
