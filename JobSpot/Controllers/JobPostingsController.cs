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
        private readonly IUserManager _userManager;
        private ILogger<JobPostingsController> _logger;

        public JobPostingsController(IRepository<JobPosting> repository, IUserManager userManager, ILogger<JobPostingsController> logger)
        {
            _jobPostingRepository = repository;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Employer"))
            {
                var userId = _userManager.GetUserId(User);
                var allJobPostings = await _jobPostingRepository.GetAllAsync();
                var userJobPostings = allJobPostings.Where(jp => jp.UserId == userId);
                _logger.LogInformation("Employer {UserId} accessed their job postings.", userId);
                return View(userJobPostings);
            }

            var jobPostings = await _jobPostingRepository.GetAllAsync();
            return View(jobPostings);
        }

        [Authorize(Policy = "CanCreateJobPosting")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateJobPosting")]
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
                    UserId = _userManager.GetUserId(User)
                };

                await _jobPostingRepository.AddAsync(jobPosting);
                _logger.LogInformation("Job posting created: {JobTitle} by User: {UserId}", jobPosting.Title, jobPosting.UserId);
                return RedirectToAction(nameof(Index));
                //    jobPosting.IsApproved = true; // Auto-approve for simplicity
                //    var user = await _userManager.GetUserAsync(User);
                //    jobPosting.UserId = user?.Id ?? "Anonymous"; // Assign UserId or "Anonymous"
                //    await _jobPostingRepository.AddAsync(jobPosting);
            } 
                catch(Exception ex)
                { 
                    _logger.LogError(ex, "Error creating job posting.");
                    return View(jobPostingVM);
                }
            }

            return View(jobPostingVM);
        }

        [Authorize(Policy = "CanEditJobPosting")]
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
                Location = jobPosting.Location
                // Add other properties as needed
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "CanEditJobPosting")]
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

            // Update fields
            jobPosting.Title = jobPostingViewModel.Title;
            jobPosting.Description = jobPostingViewModel.Description;
            jobPosting.Company = jobPostingViewModel.Company;
            jobPosting.Location = jobPostingViewModel.Location;
            // Add other fields as needed

            await _jobPostingRepository.UpdateAsync(jobPosting);
            _logger.LogInformation("Job posting updated: {JobTitle} by User: {UserId}", jobPosting.Title, jobPosting.UserId);

            return RedirectToAction(nameof(Index));
        }


        [HttpDelete]
        [Authorize(Policy = "CanDeleteJobPosting")]
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
