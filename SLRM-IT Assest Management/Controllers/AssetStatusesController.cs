using Microsoft.AspNetCore.Mvc;
using AssetManagement.Data;
using SLRM_IT_Assest_Management.Models;
using System.Linq;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class AssetStatusesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetStatusesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var statuses = _context.AssetStatuses.ToList();
            return View(statuses);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Status status)
        {
            if (status == null || string.IsNullOrWhiteSpace(status.Name))
                return BadRequest("Invalid status.");

            _context.AssetStatuses.Add(status);
            _context.SaveChanges();
            return Json(status);
        }
    }
}
