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
            ViewBag.Components = await _context.Components.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();

            return View();
        }

        // POST: ComponentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(Component component)
        {
            if (ModelState.IsValid)
            {
                _context.Components.Add(component);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Components added!";
                return RedirectToAction("Create", "Components");
            }

            ViewBag.Category = _context.Categories.ToList();
            ViewBag.AssetLocations = _context.AssetLocations.ToList();



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
