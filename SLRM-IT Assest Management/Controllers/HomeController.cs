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

            var licenses = await _context.Licenses.ToListAsync();

            // ? ADD THIS
            var consumablesCount = await _context.Consumables.CountAsync();

            var model = new DashboardViewModel
            {
                AssetCount = assets.Count,
                LicenseCount = licenses.Count,

                // ? SET HERE
                ConsumablesCount = consumablesCount
            };

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

            // Laptop breakdown
            model.LaptopWorking = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "WORKING");
            model.LaptopUnderRepair = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "UNDER REPAIR");
            model.LaptopScrap = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "SCRAP");
            model.LaptopMissing = laptops.Count(a => a.AssetStatus?.Name?.Trim().ToUpper() == "MISSING");

            // Desktop breakdown
            model.DesktopWorking = desktops.Count(a => a.AssetStatus?.Name?.Equals("Working", StringComparison.OrdinalIgnoreCase) == true);
            model.DesktopUnderRepair = desktops.Count(a => a.AssetStatus?.Name?.Equals("Under Repair", StringComparison.OrdinalIgnoreCase) == true);
            model.DesktopScrap = desktops.Count(a => a.AssetStatus?.Name?.Equals("Scrap", StringComparison.OrdinalIgnoreCase) == true);
            model.DesktopMissing = desktops.Count(a => a.AssetStatus?.Name?.Equals("Missing", StringComparison.OrdinalIgnoreCase) == true);

            return View(model);
        }

    }
}
