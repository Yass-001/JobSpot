using JobSpot.Models;
using JobSpot.Repositories;
using JobSpot.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobSpot.Controllers
{
    [Authorize]
    public class JobPostingsController : Controller
    {
        private readonly IRepository<JobPosting> _jobPostingRepository;
        private readonly UserManager<IdentityUser> _userManager; // IUserManager injection - ?!

        public JobPostingsController(IRepository<JobPosting> repository, UserManager<IdentityUser> userManager)
        {
            _jobPostingRepository = repository;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var jobPostings = await _jobPostingRepository.GetAllAsync(); // IEnumerable<JobPosting>
            return View(jobPostings);
        }

        [Authorize(Roles = "Admin,Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create(JobPostingViewModel jobPostingVM)
        {
            if (ModelState.IsValid)
            {
                var jobPosting = new JobPosting
                {
                    Title = jobPostingVM.Title,
                    Description = jobPostingVM.Description,
                    Company = jobPostingVM.Company,
                    Location = jobPostingVM.Location,
                    UserId = _userManager.GetUserId(User)
                };

                await _jobPostingRepository.AddAsync(jobPosting);
                return RedirectToAction(nameof(Index));

                //    jobPosting.IsApproved = true; // Auto-approve for simplicity
                //    var user = await _userManager.GetUserAsync(User);
                //    jobPosting.UserId = user?.Id ?? "Anonymous"; // Assign UserId or "Anonymous"
                //    await _jobPostingRepository.AddAsync(jobPosting);
            }
            return View(jobPostingVM);
        }

        [HttpDelete]
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
            }
            else
            {
                return Forbid();
            }

            return Ok();
        }


        //[Authorize(Roles = "Admin,Employer")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var jobPosting = await _jobPostingRepository.GetByIdAsync(id);
        //    if (jobPosting == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(jobPosting);
        //}

        //[HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken] // Uncomment if CSRF protection is needed
        //[Authorize(Roles = "Admin,Employer")]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var jobPosting = await _jobPostingRepository.GetByIdAsync(id);
        //    if (jobPosting == null)
        //    {
        //        return NotFound();
        //    }
        //    await _jobPostingRepository.DeleteAsync(jobPosting.Id);
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
