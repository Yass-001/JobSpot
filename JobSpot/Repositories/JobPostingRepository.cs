using JobSpot.Data;
using JobSpot.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.X86;

namespace JobSpot.Repositories
{
    public class JobPostingRepository : IRepository<JobPosting>
    {
        private readonly AppDbContext _context;

        public JobPostingRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task AddAsync(JobPosting entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "JobPosting entity cannot be null");
            }
            await _context.JobPostings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting != null)
            {
                _context.JobPostings.Remove(jobPosting);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"JobPosting with Id {id} not found.");
            }
        }

        public async Task<IEnumerable<JobPosting>> GetAllAsync()
        {
            return await _context.JobPostings.ToListAsync();
        }

        public async Task<JobPosting> GetByIdAsync(Guid id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting != null)
            {
                return jobPosting;
            }
            throw new KeyNotFoundException($"JobPosting with Id {id} not found.");
        }

        public async Task UpdateAsync(JobPosting entity)
        {
            _context.JobPostings.Update(entity);
            //await _context.JobPostings
            //    .Where(jp => jp.Id == entity.Id)
            //    .ExecuteUpdateAsync(jp => jp
            //        .SetProperty(jp => jp.Title, entity.Title)
            //        .SetProperty(jp => jp.Description, entity.Description)
            //        .SetProperty(jp => jp.Company, entity.Company)
            //        .SetProperty(jp => jp.Location, entity.Location)
            //        .SetProperty(jp => jp.IsApproved, entity.IsApproved));
            // check the properties to be updated - ?!
            await _context.SaveChangesAsync();

            //_context.Entry(jobPosting).CurrentValues.SetValues(entity);
            //await _context.SaveChangesAsync();
        }
    }
}

//v.1
//_context.JobPostings.Update(entity);
//await _context.SaveChangesAsync();
//v.2
//await _context.JobPostings
//    .Where(jp => jp.Id == entity.Id)
//    .ExecuteUpdateAsync(jp => jp
//        .SetProperty(jp => jp.Title, entity.Title)
//        .SetProperty(jp => jp.Description, entity.Description)
//        .SetProperty(jp => jp.Company, entity.Company)
//        .SetProperty(jp => jp.Location, entity.Location)
//        .SetProperty(jp => jp.IsApproved, entity.IsApproved));
//await _context.SaveChangesAsync();
//v.3
//var existing = await _context.JobPostings.FindAsync(entity.Id);
//if (existing == null)
//    throw new KeyNotFoundException($"JobPosting with ID {entity.Id} not found.");

//_context.Entry(existing).CurrentValues.SetValues(entity);
//await _context.SaveChangesAsync();


// what is the best way to implement UpdateAsync method?
// what are the pros and cons of each approach?
//•	Use SetValues() (v3) → safest and most EF-friendly for standard updates.
//•	Use ExecuteUpdateAsync() (v2) → best for performance-critical or bulk operations.
//•	Use Update() (v1) → fine for simple detached entity updates, but less efficient.
