// Changed for auth claims, previous version before change: 0.4.5
// wasn`t done

using JobSpot.Interfaces;
using JobSpot.Models;
using JobSpot.Repositories;
using JobSpot.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace JobSpot.Controllers
{
    [Authorize]
    public class JobPostingsController : Controller
    {
        private readonly IRepository<JobPosting> _jobPostingRepository;
        private readonly JobPostingRepository _jobPostingRepositoryConcrete; // For access to custom methods
        private readonly IUserManager _userManager;
        private ILogger<JobPostingsController> _logger;

        public JobPostingsController(IRepository<JobPosting> repository, JobPostingRepository jobPostingRepositoryConcrete, IUserManager userManager, ILogger<JobPostingsController> logger)
        {
            _jobPostingRepository = repository;
            _jobPostingRepositoryConcrete = jobPostingRepositoryConcrete;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Display job listings with advanced search, filtering, sorting, and pagination
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string searchQuery = null,
            string category = null,
            decimal? salaryMin = null,
            decimal? salaryMax = null,
            int? postedWithinDays = null,
            JobSortOption sortBy = JobSortOption.Newest,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                // If user is Employer, show only their postings (without search/filter)
                if (User.IsInRole("Employer"))
                {
                    var userId = _userManager.GetUserId(User);
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("GetUserId returned null for an Employer role user");
                        return RedirectToAction("Login", "Account");
                    }

                    var allJobPostings = await _jobPostingRepository.GetAllAsync();
                    var userJobPostings = allJobPostings.Where(jp => jp.UserId == userId).ToList();
                    _logger.LogInformation("Employer {UserId} accessed their job postings.", userId);

                    return View(new PaginatedResult<JobPosting>
                    {
                        Items = userJobPostings,
                        TotalCount = userJobPostings.Count,
                        PageNumber = 1,
                        PageSize = userJobPostings.Count
                    });
                }

                // For job seekers/public: perform search, filter, and pagination
                var result = await _jobPostingRepositoryConcrete.SearchAndFilterAsync(
                    searchQuery: searchQuery,
                    category: category,
                    salaryMin: salaryMin,
                    salaryMax: salaryMax,
                    postedWithinDays: postedWithinDays,
                    sortOption: sortBy,
                    pageNumber: pageNumber,
                    pageSize: pageSize);

                // Get available categories for filter dropdown
                var categories = await _jobPostingRepositoryConcrete.GetCategoriesAsync();

                // Create filter view model with current filter state
                var filterModel = new JobSearchFilterViewModel
                {
                    SearchQuery = searchQuery,
                    Category = category,
                    SalaryMin = salaryMin,
                    SalaryMax = salaryMax,
                    PostedWithinDays = postedWithinDays,
                    SortBy = sortBy,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    AvailableCategories = categories
                };

                // Pass filter model to view via ViewBag
                ViewBag.FilterModel = filterModel;

                _logger.LogInformation("Displaying job listings - Page {PageNumber}, Search: {SearchQuery}, Category: {Category}", 
                    pageNumber, searchQuery, category);

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job postings");
                return View(new PaginatedResult<JobPosting>());
            }
        }

        //[Authorize(Policy = "CanCreateJobPosting")]
        [Authorize(Roles = "Admin,Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Policy = "CanCreateJobPosting")]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create(JobPostingViewModel jobPostingVM)
        {
            if (ModelState.IsValid)
            {
                try {
                var jobPosting = new JobPosting
                {
                    Title = jobPostingVM.Title,
                    Description = jobPostingVM.Description,
                    Company = jobPostingVM.Company,
                    Location = jobPostingVM.Location,
                    Category = jobPostingVM.Category,
                    SalaryMin = jobPostingVM.SalaryMin,
                    SalaryMax = jobPostingVM.SalaryMax,
                    SalaryCurrency = jobPostingVM.SalaryCurrency,
                    UserId = _userManager.GetUserId(User)
                };

                await _jobPostingRepository.AddAsync(jobPosting);
                _logger.LogInformation("Job posting created: {JobTitle} by User: {UserId}", jobPosting.Title, jobPosting.UserId);
                return RedirectToAction(nameof(Index));
            } 
                catch(Exception ex)
                { 
                    _logger.LogError(ex, "Error creating job posting.");
                    return View(jobPostingVM);
                }
            }

            return View(jobPostingVM);
        }

        //[Authorize(Policy = "CanEditJobPosting")]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(Guid id) // IActionResult Edit(Guid id)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") && jobPosting.UserId != userId)
            {
                return Forbid();
            }

            var viewModel = new JobPostingViewModel
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Company = jobPosting.Company,
                Location = jobPosting.Location,
                Category = jobPosting.Category,
                SalaryMin = jobPosting.SalaryMin,
                SalaryMax = jobPosting.SalaryMax,
                SalaryCurrency = jobPosting.SalaryCurrency
            };

            return View(viewModel);
        }

        [HttpPost]
        //[Authorize(Policy = "CanEditJobPosting")]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(JobPostingViewModel jobPostingViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(jobPostingViewModel);
            }

            var jobPosting = await _jobPostingRepository.GetByIdAsync(jobPostingViewModel.Id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") && jobPosting.UserId != userId)
            {
                return Forbid();
            }

            // Update all fields including new ones
            jobPosting.Title = jobPostingViewModel.Title;
            jobPosting.Description = jobPostingViewModel.Description;
            jobPosting.Company = jobPostingViewModel.Company;
            jobPosting.Location = jobPostingViewModel.Location;
            jobPosting.Category = jobPostingViewModel.Category;
            jobPosting.SalaryMin = jobPostingViewModel.SalaryMin;
            jobPosting.SalaryMax = jobPostingViewModel.SalaryMax;
            jobPosting.SalaryCurrency = jobPostingViewModel.SalaryCurrency;

            await _jobPostingRepository.UpdateAsync(jobPosting);
            _logger.LogInformation("Job posting updated: {JobTitle} by User: {UserId}", jobPosting.Title, jobPosting.UserId);

            return RedirectToAction(nameof(Index));
        }



        [HttpDelete]
        //[Authorize(Policy = "CanDeleteJobPosting")]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin") || jobPosting.UserId == userId)
            {
                await _jobPostingRepository.DeleteAsync(id);
                _logger.LogInformation("Job posting deleted: {JobTitle} by User: {UserId}", jobPosting.Title, userId);
            }
            else
            {
                return Forbid();
            }

            return Ok();
        }
    }
}
