using Microsoft.AspNetCore.Mvc;
using AssetManagement.Data;   // your DbContext namespace
using SLRM_IT_Assest_Management.Models;
using System.Linq;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class AssetTypesController : Controller
    {
        // 1️⃣ Declare _context as a private readonly field
        private readonly ApplicationDbContext _context;

        // 2️⃣ Initialize _context via constructor injection
        public AssetTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Example: Create action
        [HttpPost]
        public IActionResult Create([FromBody] AssetType assetType)
        {
            if (assetType == null || string.IsNullOrWhiteSpace(assetType.Name))
            {
                return BadRequest("Invalid asset type name.");
            }

            _context.AssetTypes.Add(assetType);
            _context.SaveChanges();

            return Json(new { success = true, assetTypeId = assetType.AssetTypeId, name = assetType.Name });
        }

        // Example: Get all asset types
        [HttpGet]
        public IActionResult GetAll()
        {
            var assetTypes = _context.AssetTypes.Select(a => new { a.AssetTypeId, a.Name }).ToList();
            return Json(assetTypes);
        }
    }
}
