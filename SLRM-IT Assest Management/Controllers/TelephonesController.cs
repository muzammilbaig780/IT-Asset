using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class TelephonesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TelephonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Telephones
        public async Task<IActionResult> Index()
        {
            var telephone = await _context.Telephone
                //.Include(t => t.AssetLocation)  // Eager load AssetLocation
                .Include(t => t.Department)      // Eager load Department
                .ToListAsync();

            return View(telephone);
        }

        // GET: Telephones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telephone = await _context.Telephone
                .FirstOrDefaultAsync(m => m.TelephoneId == id);
            if (telephone == null)
            {
                return NotFound();
            }

            return View(telephone);
        }

        // GET: Telephones/Create
        public IActionResult Create()
        {
            ViewData["Departments"] = _context.Departments.ToList();
            return View();
        }

        // POST: Telephones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TelephoneId,UserName,TelephoneNo,DepartmentId,Connection")] Telephone telephone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(telephone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(telephone);
        }

        // GET: Telephones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telephone = await _context.Telephone.FindAsync(id);
            if (telephone == null)
            {
                return NotFound();
            }

            ViewData["Departments"] = await _context.Departments.ToListAsync();
            return View(telephone);
        }

        // POST: Telephones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TelephoneId,UserName,TelephoneNo,DepartmentId,Connection")] Telephone telephone)
        {
            if (id != telephone.TelephoneId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telephone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelephoneExists(telephone.TelephoneId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(telephone);
        }

        // GET: Telephones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telephone = await _context.Telephone
                .FirstOrDefaultAsync(m => m.TelephoneId == id);
            if (telephone == null)
            {
                return NotFound();
            }

            return View(telephone);
        }

        // POST: Telephones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telephone = await _context.Telephone.FindAsync(id);
            if (telephone != null)
            {
                _context.Telephone.Remove(telephone);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelephoneExists(int id)
        {
            return _context.Telephone.Any(e => e.TelephoneId == id);
        }

        // ================= IMPORT TELEPHONES =================

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file, bool overwriteExisting = false)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid Excel file.";
                return RedirectToAction(nameof(Import));
            }

            var telephones = new List<Telephone>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        if (worksheet == null || worksheet.Dimension == null)
                            throw new Exception("The Excel file is empty or invalid.");

                        int rowCount = worksheet.Dimension.Rows;

                        // Skip header row
                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1]?.Text))
                                continue;

                            string userName = worksheet.Cells[row, 1]?.Text?.Trim();
                            string telephoneNo = worksheet.Cells[row, 2]?.Text?.Trim();
                            string departmentText = worksheet.Cells[row, 3]?.Text?.Trim();
                            string connection = worksheet.Cells[row, 4]?.Text?.Trim();

                            // ===== Ensure Department exists =====
                            var department = await _context.Departments
                                .FirstOrDefaultAsync(d => d.DepartmentName == departmentText);

                            if (department == null)
                            {
                                department = new Department
                                {
                                    DepartmentName = string.IsNullOrEmpty(departmentText)
                                        ? "Unknown"
                                        : departmentText
                                };

                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }

                            // ===== Create Telephone object =====
                            var telephone = new Telephone
                            {
                                UserName = userName,
                                TelephoneNo = telephoneNo,
                                DepartmentId = department.DepartmentId,
                                Connection = connection
                            };

                            telephones.Add(telephone);
                        }
                    }
                }

                // ===== Overwrite existing data =====
                if (overwriteExisting)
                {
                    _context.Telephone.RemoveRange(_context.Telephone);
                    await _context.SaveChangesAsync();
                }

                // ===== Insert imported data =====
                if (telephones.Any())
                {
                    await _context.Telephone.AddRangeAsync(telephones);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] =
                    $"Successfully imported {telephones.Count} telephones.";

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
