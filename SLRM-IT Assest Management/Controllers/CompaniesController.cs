using Microsoft.AspNetCore.Mvc;
using AssetManagement.Data;
using SLRM_IT_Assest_Management.Models;
using System.Linq;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var companies = _context.Companies.ToList();
            return View(companies);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Company company)
        {
            if (company == null || string.IsNullOrWhiteSpace(company.CompanyName))
                return BadRequest("Invalid company.");

            _context.Companies.Add(company);
            _context.SaveChanges();
            return Json(company);
        }
    }
}
