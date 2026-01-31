using AssetManagement.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class ComponentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ComponentController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // GET: ComponentController
        public async Task<IActionResult> Index()
        {
            var components = await _context.Components.ToListAsync();
            return View(components);
        }

        private async Task PopulateDropDownsAsync()
        {
            ViewBag.Category = await _context.Categories.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // ✅ MATCH VIEWBAG KEYS WITH VIEW EXPECTATIONS
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Component component)
        {
            if (ModelState.IsValid)
            {
                _context.Components.Add(component);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Component added successfully!";
                return RedirectToAction(nameof(Index)); // ✅ Go to Index, not Create
            }

            // ✅ SAME KEYS AS GET METHOD + ASYNC
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();

            return View(component);
        }


        // GET: ComponentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComponentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComponentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComponentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
