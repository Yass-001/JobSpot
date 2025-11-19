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

        [Fact]
        public async Task GetByIdAsync_ShouldReturnJobPosting()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new JobPostingRepository(context);
            var jobPosting = new JobPosting
            {
                Title = "Data Scientist",
                Description = "Analyze and interpret complex data.",
                Company = "Data Corp",
                Location = "San Francisco, CA",
                PostedDate = DateTime.Now,
                IsApproved = false,
                UserId = "Test UserId 2"
            };
            //await repository.AddAsync(jobPosting); // we are not testing AddAsync here, so it's fine to use DB context directly
            await context.JobPostings.AddAsync(jobPosting);
            await context.SaveChangesAsync();

            // Act
            var retrievedJobPosting = await repository.GetByIdAsync(jobPosting.Id);

            // Assert
            Assert.NotNull(retrievedJobPosting);
            Assert.Equal("Data Scientist", retrievedJobPosting.Title);
            Assert.False(retrievedJobPosting.IsApproved);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new JobPostingRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repository.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveJobPostingFromDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new JobPostingRepository(context);
            var jobPosting = new JobPosting
            {
                Title = "Project Manager",
                Description = "Manage projects and teams.",
                Company = "Management Inc",
                Location = "Chicago, IL",
                PostedDate = DateTime.Now,
                IsApproved = true,
                UserId = "Test UserId 3"
            };
            await context.JobPostings.AddAsync(jobPosting);
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.NotNull(jobPosting);
            await repository.DeleteAsync(jobPosting.Id);
            var deletedJobPosting = await context.JobPostings.FindAsync(jobPosting.Id);
            Assert.Null(deletedJobPosting);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new JobPostingRepository(context);
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repository.DeleteAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateJobPostingInDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new JobPostingRepository(context);
            var jobPosting = new JobPosting
            {
                Title = "QA Engineer",
                Description = "Test software applications.",
                Company = "Quality Corp",
                Location = "Austin, TX",
                PostedDate = DateTime.Now,
                IsApproved = false,
                UserId = "Test UserId 4"
            };
            await context.JobPostings.AddAsync(jobPosting);
            await context.SaveChangesAsync();

            // Act
            jobPosting.Title = "Senior QA Engineer";
            jobPosting.IsApproved = true;
            await repository.UpdateAsync(jobPosting);
            var updatedJobPosting = await context.JobPostings.FindAsync(jobPosting.Id);

            // Assert
            Assert.NotNull(updatedJobPosting);
            Assert.Equal("Senior QA Engineer", updatedJobPosting.Title);
            Assert.True(updatedJobPosting.IsApproved);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllJobPostings()
        {
            using var context = CreateDbContext();
            var repository = new JobPostingRepository(context);
            var jobPosting1 = new JobPosting
            {
                Title = "DevOps Engineer",
                Description = "Maintain infrastructure.",
                Company = "Infra Corp",
                Location = "Seattle, WA",
                PostedDate = DateTime.Now,
                IsApproved = true,
                UserId = "Test UserId 5"
            };
            var jobPosting2 = new JobPosting
            {
                Title = "UI/UX Designer",
                Description = "Design user interfaces.",
                Company = "Design Studio",
                Location = "Boston, MA",
                PostedDate = DateTime.Now,
                IsApproved = false,
                UserId = "Test UserId 6"
            };
            await context.JobPostings.AddRangeAsync(jobPosting1, jobPosting2);
            await context.SaveChangesAsync();

            // Act
            var allJobPostings = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(allJobPostings);
            Assert.Contains<JobPosting>(allJobPostings, jp => jp.Company == "Infra Corp");
            Assert.Equal(2, allJobPostings.Count());
            Assert.NotEqual(3, allJobPostings.Count());
            Assert.True(allJobPostings.Count() >= 2);
            //var jobPostingsList = allJobPostings.ToList();
            //Assert.Equal(2, jobPostingsList.Count);
            Assert.Contains(allJobPostings, jp => jp.Title == "DevOps Engineer");
            Assert.Contains(allJobPostings, jp => jp.Title == "UI/UX Designer");
            Assert.Collection(allJobPostings, new Action<JobPosting>[] {
                jp => Assert.Equal("DevOps Engineer", jp.Title),
                jp => Assert.Equal("UI/UX Designer", jp.Title) });

        }
    }
}
