using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using SLRM_IT_Assest_Management.Data;
using SLRM_IT_Assest_Management.Models;
using System.Text.Json;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class BarcodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarcodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var barcodes = await _context.Barcode
                .Include(b => b.AssetLocation)
                .Include(b => b.Department)
                .AsNoTracking() // Performance optimization for read-only
                .ToListAsync();

            return View(barcodes);
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var barcode = await _context.Barcode
                .Include(b => b.AssetLocation)
                .Include(b => b.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BarcodeId == id);

            if (barcode == null) return NotFound();

            return View(barcode);
        }

        // ================= CREATE =================
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Barcode barcode)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate Asset Tag
                var exists = await _context.Barcode
                    .AnyAsync(b => b.ITAssetTag == barcode.ITAssetTag);

                if (exists)
                {
                    ModelState.AddModelError("ITAssetTag", "Asset Tag already exists.");
                }
                else
                {
                    _context.Add(barcode);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Barcode <strong>{barcode.ITAssetTag}</strong> created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            await PopulateDropdowns();
            return View(barcode);
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var barcode = await _context.Barcode
                .Include(b => b.AssetLocation)
                .Include(b => b.Department)
                .FirstOrDefaultAsync(b => b.BarcodeId == id);

            if (barcode == null) return NotFound();

            await PopulateDropdowns(barcode);
            return View(barcode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Barcode barcode)
        {
            if (id != barcode.BarcodeId) return NotFound();

            // Check concurrency
            var existing = await _context.Barcode.AsNoTracking().FirstOrDefaultAsync(b => b.BarcodeId == id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for duplicate Asset Tag (excluding current record)
                    var duplicate = await _context.Barcode
                        .AnyAsync(b => b.ITAssetTag == barcode.ITAssetTag && b.BarcodeId != id);

                    if (duplicate)
                    {
                        ModelState.AddModelError("ITAssetTag", "Asset Tag already exists for another barcode.");
                    }
                    else
                    {
                        _context.Update(barcode);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = $"Barcode <strong>{barcode.ITAssetTag}</strong> updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BarcodeExists(barcode.BarcodeId))
                        return NotFound();

                    ModelState.AddModelError("", "Unable to save changes. The barcode was modified by another user.");
                }
            }

            await PopulateDropdowns(barcode);
            return View(barcode);
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var barcode = await _context.Barcode
                .Include(b => b.AssetLocation)
                .Include(b => b.Department)
                .FirstOrDefaultAsync(m => m.BarcodeId == id);

            if (barcode == null) return NotFound();

            return View(barcode);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barcode = await _context.Barcode.FindAsync(id);
            if (barcode != null)
            {
                var assetTag = barcode.ITAssetTag;
                _context.Barcode.Remove(barcode);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Barcode <strong>{assetTag}</strong> deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= PRIVATE HELPERS =================
        private async Task PopulateDropdowns(Barcode? barcode = null)
        {
            var locations = await _context.AssetLocations
                .OrderBy(l => l.Name)
                .AsNoTracking()
                .ToListAsync();

            var departments = await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.AssetLocations = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                locations, "Id", "Name", barcode?.AssetLocationId);

            ViewBag.Departments = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                departments, "Id", "DepartmentName", barcode?.DepartmentId);
        }

        private bool BarcodeExists(int id)
        {
            return _context.Barcode.Any(e => e.BarcodeId == id);
        }

        // ================= JSON ENDPOINT FOR AJAX =================
        [HttpGet]
        public async Task<IActionResult> GetBarcodes(string search = "")
        {
            var barcodes = await _context.Barcode
                .Where(b => string.IsNullOrEmpty(search) ||
                           b.ITAssetTag.Contains(search) ||
                           b.SerialNumber.Contains(search))
                .Take(10)
                .Select(b => new { b.BarcodeId, b.ITAssetTag, b.SerialNumber })
                .ToListAsync();

            return Json(barcodes);
        }
    }
}
