using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;
using System.Globalization;

namespace AssetManagement.Controllers
{
    public class LicensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LicensesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Licenses
        public async Task<IActionResult> Index()
        {
            var licenses = await _context.Licenses.ToListAsync();
            return View(licenses);
        }

        // GET: Licenses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Licenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicenseId,SerialNo, MachineOrServiceDetails, Type, Division, UserDepartment, Quantity, FromDate, ToDate, RenewalBefore, WorkOrderNo, ContactPerson,ContactNo, OfficeContactNo, EmailId, Address")] License license)
        {
            if (ModelState.IsValid)
            {
                _context.Add(license);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "License created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(license);
        }

        // GET: Licenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var license = await _context.Licenses.FindAsync(id);
            if (license == null) return NotFound();

            return View(license);
        }

        // POST: Licenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LicenseId,Name,Key,Type,AssignedTo,ExpirationDate,Status,Notes")] License license)
        {
            if (id != license.LicenseId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(license);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "License updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseExists(license.LicenseId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(license);
        }

        // GET: Licenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var license = await _context.Licenses.FirstOrDefaultAsync(m => m.LicenseId == id);
            if (license == null) return NotFound();

            return View(license);
        }

        // POST: Licenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var license = await _context.Licenses.FindAsync(id);
            if (license != null)
            {
                _context.Licenses.Remove(license);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "License deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseExists(int id)
        {
            return _context.Licenses.Any(e => e.LicenseId == id);
        }

        // GET: Licenses/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Licenses/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file, bool overwriteExisting = false)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid Excel file.";
                return RedirectToAction(nameof(Import));
            }

            var licenses = new List<License>();

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);

                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null || worksheet.Dimension == null)
                    throw new Exception("The Excel file is empty or invalid.");

                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    // Example: map columns accordingly; adjust indices as per your Excel columns
                    var serialNo = worksheet.Cells[row, 1]?.Text?.Trim();
                    if (string.IsNullOrWhiteSpace(serialNo)) continue;

                    var license = new License
                    {
                        SerialNo = serialNo,
                        MachineOrServiceDetails = worksheet.Cells[row, 2]?.Text?.Trim(),
                        Type = worksheet.Cells[row, 3]?.Text?.Trim(),
                        Division = worksheet.Cells[row, 4]?.Text?.Trim(),
                        UserDepartment = worksheet.Cells[row, 5]?.Text?.Trim(),

                        Quantity = int.TryParse(worksheet.Cells[row, 6]?.Text?.Trim(), out var qty) ? qty : 0,

                        FromDate = DateTime.TryParse(worksheet.Cells[row, 7]?.Text?.Trim(), out var fromDate) ? fromDate : DateTime.MinValue,
                        ToDate = DateTime.TryParse(worksheet.Cells[row, 8]?.Text?.Trim(), out var toDate) ? toDate : DateTime.MinValue,

                        WorkOrderNo = worksheet.Cells[row, 9]?.Text?.Trim(),

                        RenewalBefore = DateTime.TryParse(worksheet.Cells[row, 10]?.Text?.Trim(), out var renewalBefore) ? renewalBefore : DateTime.MinValue,

                        ContactPerson = worksheet.Cells[row, 11]?.Text?.Trim(),
                        ContactNo = worksheet.Cells[row, 12]?.Text?.Trim(),
                        OfficeContactNo = worksheet.Cells[row, 13]?.Text?.Trim(),
                        EmailId = worksheet.Cells[row, 14]?.Text?.Trim(),
                        Address = worksheet.Cells[row, 15]?.Text?.Trim()
                    };

                    licenses.Add(license);
                }

                if (overwriteExisting)
                    _context.Licenses.RemoveRange(_context.Licenses);

                if (licenses.Any())
                {
                    await _context.Licenses.AddRangeAsync(licenses);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Imported {licenses.Count} licenses successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Import failed: " + ex.Message;
                return RedirectToAction(nameof(Import));
            }
        }

    }
}
