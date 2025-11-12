using JobSpot.Data;
using JobSpot.Models;

namespace JobSpot.Repositories
{
    public class JobPostingRepository : IRepository<JobPosting>
    {
        private readonly AppDbContext _context;
        public JobPostingRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public Task AddAsync(JobPosting entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JobPosting>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobPosting> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(JobPosting entity)
        {
            throw new NotImplementedException();
        }
    }
}
