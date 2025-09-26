using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using SLRM_IT_Assest_Management.Models;
using System.Diagnostics;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext through constructor
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboardData = new DashbaordViewModel
            {
                AssetCount = _context.Assets.Count(),   // <-- Actual count from DB
                LicenseCount = _context.Licenses.Count(),
                //AccessoriesCount = _context.Accessories.Count(),
                //ConsumablesCount = _context.Consumables.Count()
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
