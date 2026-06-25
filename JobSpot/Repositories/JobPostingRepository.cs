using JobSpot.Data;
using JobSpot.Models;
using JobSpot.ViewModels;
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

        public JobPostingRepository()
        {
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
            try
            {
                return await _context.JobPostings.AsNoTracking().ToListAsync(); //    return await _context.JobPostings.AsNoTracking().ToListAsync();
            }
            catch (OperationCanceledException)
            {
                throw new Exception("The operation was canceled. Ensure the request scope is still active.");
            }
            catch (ObjectDisposedException ex)
            {
                throw new InvalidOperationException("DbContext has been disposed. Ensure the request scope is still active.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving job postings.", ex);
            }
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
            var jobPostingForUpdate = await _context.JobPostings.FindAsync(entity.Id);
            if (jobPostingForUpdate == null)
            {
                throw new KeyNotFoundException($"JobPosting with Id {entity.Id} not found.");
            }
            _context.Entry(jobPostingForUpdate).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ViewModels.PaginatedResult<JobPosting>> SearchAndFilterAsync(
            string? searchQuery = null,
            string? category = null,
            decimal? salaryMin = null,
            decimal? salaryMax = null,
            int? postedWithinDays = null,
            JobSortOption sortOption = JobSortOption.Newest,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.JobPostings.AsQueryable();

            // Filter by search query
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(jp => (jp.Title ?? "").Contains(searchQuery) || 
                                          (jp.Description ?? "").Contains(searchQuery) ||
                                          (jp.Company ?? "").Contains(searchQuery) ||
                                          (jp.Location ?? "").Contains(searchQuery));
            }

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(jp => jp.Category == category);
            }

            // Filter by salary range
            if (salaryMin.HasValue)
            {
                query = query.Where(jp => jp.SalaryMax >= salaryMin);
            }

            if (salaryMax.HasValue)
            {
                query = query.Where(jp => jp.SalaryMin <= salaryMax);
            }

            // Filter by posted within days
            if (postedWithinDays.HasValue)
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-postedWithinDays.Value);
                query = query.Where(jp => jp.PostedDate >= cutoffDate);
            }

            // Apply sorting
            query = sortOption switch
            {
                JobSortOption.Newest => query.OrderByDescending(jp => jp.PostedDate),
                JobSortOption.Oldest => query.OrderBy(jp => jp.PostedDate),
                JobSortOption.SalaryHighToLow => query.OrderByDescending(jp => jp.SalaryMax),
                JobSortOption.SalaryLowToHigh => query.OrderBy(jp => jp.SalaryMin),
                JobSortOption.CompanyAZ => query.OrderBy(jp => jp.Company),
                JobSortOption.CompanyZA => query.OrderByDescending(jp => jp.Company),
                JobSortOption.TitleAZ => query.OrderBy(jp => jp.Title),
                JobSortOption.TitleZA => query.OrderByDescending(jp => jp.Title),
                _ => query.OrderByDescending(jp => jp.PostedDate)
            };

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<JobPosting>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return (await _context.JobPostings
                .Select(jp => jp.Category)
                .Where(c => c != null)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync()).Cast<string>();
        }
        //public async Task<IEnumerable<string>> GetCategoriesAsync()
        //{
        //    return await _context.JobPostings
        //    return await _context.JobPostings
        //        .Select(jp => jp.Category)
        //        .Where(c => c != null)
        //        .Distinct()
        //        .OrderBy(c => c)
        //        .ToListAsync();
        //}

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
// v.2 wasn`t working because of LangVersion 13.0 - ExecuteUpdateAsync requires C# 14.0 or higher.
