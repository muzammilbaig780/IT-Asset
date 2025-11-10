using AssetManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;
using System.Diagnostics;
using System.Linq;

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
            var activeCount = _context.Assets
             .Include(a => a.AssetStatus)
             .Count(a => a.AssetStatus.Name == "Active");

            var notActiveCount = _context.Assets
                .Include(a => a.AssetStatus)
                .Count(a => a.AssetStatus.Name == "Not Active");

            var scrapCount = _context.Assets
    .Include(a => a.AssetStatus)
    .Count(a => a.AssetStatus.Name == "Scrap");

            var naCount = _context.Assets
   .Include(a => a.AssetStatus)
   .Count(a => a.AssetStatus.Name == "NA");

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
                LaptopCount = _context.Assets.Count(a => a.AssetType.Name == "Laptop"),
                DesktopCount = _context.Assets.Count(a => a.AssetType.Name == "Desktop"),
                ActiveCount = activeCount,
                NotActiveCount = notActiveCount,
                ScrapCount = scrapCount,                //ActiveAssetCount = _context.Assets.Count(),
                NACount = naCount,                //ActiveAssetCount = _context.Assets.Count(),
                //InactiveAssetCount = _context.Assets.Count(),
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
