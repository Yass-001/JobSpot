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

        public IActionResult Index()
        {
            return View();
        }
    }
}
