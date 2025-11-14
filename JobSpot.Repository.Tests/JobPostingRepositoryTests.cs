using JobSpot.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSpot.Repository.Tests
{
    internal class JobPostingRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public JobPostingRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb-{Guid.NewGuid()}")
                .Options;
        }

        private AppDbContext CreateDbContext() => new AppDbContext(_dbContextOptions);
    }
}
 