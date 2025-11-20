using JobSpot.Models;
using JobSpot.Repositories;
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
        public async Task<IActionResult> Create(JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                jobPosting.PostedDate = DateTime.Now;
                jobPosting.IsApproved = true; // Auto-approve for simplicity
                var user = await _userManager.GetUserAsync(User);
                jobPosting.UserId = user?.Id ?? "Anonymous"; // Assign UserId or "Anonymous"
                await _jobPostingRepository.AddAsync(jobPosting);
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }


    }
}
