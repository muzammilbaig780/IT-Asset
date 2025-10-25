using Microsoft.AspNetCore.Mvc;
using AssetManagement.Data;
using SLRM_IT_Assest_Management.Models;
using System.Linq;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class AssetLocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetLocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var locations = _context.AssetLocations.ToList();
            return View(locations);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AssetLocation location)
        {
            if (location == null || string.IsNullOrWhiteSpace(location.Name))
                return BadRequest("Invalid location.");

            _context.AssetLocations.Add(location);
            _context.SaveChanges();
            return Json(location);
        }
    }
}
