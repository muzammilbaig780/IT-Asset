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

        public async Task<IActionResult> Index()
        {
            var assets = await _context.Assets
                .Include(a => a.AssetType)
                .Include(a => a.AssetStatus)
                .ToListAsync();

            var licenses = await _context.Licenses.ToListAsync(); // ? Add this line

             

            var model = new DashboardViewModel();
            model.LicenseCount = licenses.Count;

            // Safe filtering for laptops and desktops
            var laptops = assets
                .Where(a => a.AssetType != null &&
                            a.AssetType.Name.Contains("laptop", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var desktops = assets
                .Where(a => a.AssetType != null &&
                            a.AssetType.Name.Contains("desktop", StringComparison.OrdinalIgnoreCase))
                .ToList();

            model.LaptopCount = laptops.Count;
            model.DesktopCount = desktops.Count;
            model.AssetCount = assets.Count;

            // Laptop breakdown
            model.LaptopActive = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "ACTIVE");
            model.LaptopNotActive = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "NOT ACTIVE");
            model.LaptopScrap = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "SCRAP");
            model.LaptopNA = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "NA");

            // Desktop breakdown
            model.DesktopActive = desktops.Count(a => a.AssetStatus != null &&
                a.AssetStatus.Name.Equals("Active", StringComparison.OrdinalIgnoreCase));

            model.DesktopNotActive = desktops.Count(a => a.AssetStatus != null &&
                a.AssetStatus.Name.Equals("Not Active", StringComparison.OrdinalIgnoreCase));

            model.DesktopScrap = desktops.Count(a => a.AssetStatus != null &&
                a.AssetStatus.Name.Equals("Scrap", StringComparison.OrdinalIgnoreCase));

            model.DesktopNA = desktops.Count(a => a.AssetStatus != null &&
                a.AssetStatus.Name.Equals("NA", StringComparison.OrdinalIgnoreCase));

            return View(model);
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
