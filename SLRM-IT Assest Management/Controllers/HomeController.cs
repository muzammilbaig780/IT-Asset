using AssetManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLRM_IT_Assest_Management.Models;
using System.Linq;
using System.Diagnostics;

namespace SLRM_IT_Assest_Management.Controllers
{
    [Authorize] // Only logged-in users can access
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Main dashboard page
        public IActionResult Index()
        {
            // Get the logged-in username from the cookie
            var username = User.Identity.Name;

            // Retrieve user details
            var loggedInUser = _context.Users.FirstOrDefault(u => u.Username == username);

            // Build the dashboard view model
            var dashboardData = new DashboardViewModel
            {
                AssetCount = _context.Assets.Count(),
                LicenseCount = _context.Licenses.Count(),
                ReadyToDeployCount = _context.Assets.Count(a => a.StatusId == 1),
                ActiveCount = _context.Assets.Count(a => a.StatusId == 2),
                NotActiveCount = _context.Assets.Count(a => a.StatusId == 3),
                LoggedInUser = loggedInUser
            };

            return View(dashboardData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
