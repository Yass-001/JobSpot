using JobSpot.Models;
using JobSpot.Repositories;
using JobSpot.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobSpot.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly IRepository<JobPosting> _jobPostingRepository;
        private readonly UserManager<IdentityUser> _userManager; // IUserManager injection - ?!

        public JobPostingsController(IRepository<JobPosting> repository, UserManager<IdentityUser> userManager)
        {
            _jobPostingRepository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var jobPostings = await _jobPostingRepository.GetAllAsync(); // IEnumerable<JobPosting>
            return View(jobPostings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken] // Prevent CSRF attacks - ?!
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

                //    jobPosting.PostedDate = DateTime.Now;
                //    jobPosting.IsApproved = true; // Auto-approve for simplicity
                //    var user = await _userManager.GetUserAsync(User);
                //    jobPosting.UserId = user?.Id ?? "Anonymous"; // Assign UserId or "Anonymous"
                //    await _jobPostingRepository.AddAsync(jobPosting);
                //    return RedirectToAction(nameof(Index));
            }
            //return View(jobPosting);

            return RedirectToAction(nameof(Index));
        }


    }
}
