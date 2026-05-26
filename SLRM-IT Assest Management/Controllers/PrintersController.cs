using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;
using System.Diagnostics;
using System.Globalization;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class PrintersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrintersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Printers
        public async Task<IActionResult> Index()
        {
            var printers = await _context.Printers
                .Include(p => p.PrinterType)
                .Include(p => p.AssetLocation)
                .Include(p => p.Department)
                .OrderByDescending(p => p.PrinterId)
                .AsNoTracking()
                .ToListAsync();

            return View(printers);
        }




        // GET: Printers/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Printers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Printer printer)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                // Re-populate dropdowns before returning the view
                await PopulateDropdownsAsync();
                return View(printer);
            }

            try
            {
                // Optional: Check for duplicate SerialNumber
                var existingPrinter = await _context.Printers
                    .FirstOrDefaultAsync(p => p.SerialNumber == printer.SerialNumber);

                if (existingPrinter != null)
                {
                    ModelState.AddModelError("SerialNumber", "A printer with this serial number already exists.");
                    await PopulateDropdownsAsync();
                    return View(printer);
                }

                _context.Add(printer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Printer added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving printer: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the printer.");
                await PopulateDropdownsAsync();
                return View(printer);
            }
        }

        // Helper method to populate dropdowns
        private async Task PopulateDropdownsAsync()
        {
            var printerTypes = await _context.PrinterTypes.ToListAsync() ?? new List<PrinterType>();
            var assetLocations = await _context.AssetLocations.ToListAsync() ?? new List<AssetLocation>();
            var departments = await _context.Departments.ToListAsync() ?? new List<Department>();

            ViewData["PrinterTypes"] = printerTypes;
            ViewData["AssetLocations"] = assetLocations;
            ViewData["Departments"] = departments;
        }



        // GET: Printers/Edit/5
        // GET: Printers/Edit/5
        // Edit GET action
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound(); // Handle invalid ID
            }

            var printer = await _context.Printers
                .Include(p => p.PrinterType)
                .Include(p => p.AssetLocation)
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.PrinterId == id);

            if (printer == null)
            {
                return NotFound();
            }

            // Pass data to the view
            ViewData["PrinterTypes"] = await _context.PrinterTypes.ToListAsync();
            ViewData["Locations"] = await _context.AssetLocations.ToListAsync();
            ViewData["Departments"] = await _context.Departments.ToListAsync();

            return View(printer);
        }




        // POST: Printers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrinterId,ITAssetTag,Division,PrinterTypeId,AssetLocationId,DepartmentId,PrinterMake,PrinterModel,SerialNumber,CartridgeType,Status,GRNNumber,GRNDate,InvoiceDate,Warranty,EndDate")] Printer printer)
        {
            if (id != printer.PrinterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(printer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrinterExists(printer.PrinterId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // After saving, redirect to Index or Details
                return RedirectToAction(nameof(Index));  // Or RedirectToAction(nameof(Details), new { id = printer.PrinterId });
            }
            return View(printer);
        }


        private bool PrinterExists(int id)
        {
            return _context.Printers.Any(e => e.PrinterId == id);
        }



        // GET: Printers/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var printer = _context.Printers
                .FirstOrDefault(m => m.PrinterId == id);
            if (printer == null)
            {
                return NotFound();
            }

            return View(printer);
        }

        // POST: Printers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var printer = _context.Printers.Find(id);
            _context.Printers.Remove(printer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private DateTime? SafeExcelDate(ExcelRange cell)
        {
            if (cell == null || string.IsNullOrWhiteSpace(cell.Text))
                return null;

            // Excel numeric date (most common)
            if (cell.Value is double d)
                return DateTime.FromOADate(d);

            // Try parsing text date
            if (DateTime.TryParse(cell.Text, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime parsedDate))
                return parsedDate;

            // Text like "NEED TO CHECK"
            return null;
        }


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

            var printers = new List<Printer>();

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

                        for (int row = 2; row <= rowCount; row++) // skip header row
                        {
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1]?.Text)) continue;

                            string itAssetTag = worksheet.Cells[row, 1]?.Text?.Trim();
                            string printerMake = worksheet.Cells[row, 2]?.Text?.Trim();
                            string printerModel = worksheet.Cells[row, 3]?.Text?.Trim();
                            string serialNumber = worksheet.Cells[row, 4]?.Text?.Trim();
                            string cartridgeType = worksheet.Cells[row, 5]?.Text?.Trim();
                            string printerTypeText = worksheet.Cells[row, 6]?.Text?.Trim();
                            string assetLocationText = worksheet.Cells[row, 7]?.Text?.Trim();
                            string departmentText = worksheet.Cells[row, 8]?.Text?.Trim();
                            string division = worksheet.Cells[row, 9]?.Text?.Trim();
                            string statusText = worksheet.Cells[row, 10]?.Text?.Trim();
                            string warranty = worksheet.Cells[row, 11]?.Text?.Trim();
                            string grnDateText = worksheet.Cells[row, 12]?.Text?.Trim();
                            string grnNumber = worksheet.Cells[row, 13]?.Text?.Trim();
                            string invoiceDateText = worksheet.Cells[row, 14]?.Text?.Trim();
                            string endDateText = worksheet.Cells[row, 15]?.Text?.Trim();

                            // ===== Ensure master data exists =====

                            // PrinterType
                            var printerType = await _context.PrinterTypes
                                .FirstOrDefaultAsync(a => a.Name == printerTypeText);
                            if (printerType == null)
                            {
                                printerType = new PrinterType
                                {
                                    Name = string.IsNullOrEmpty(printerTypeText) ? "Unknown" : printerTypeText
                                };
                                _context.PrinterTypes.Add(printerType);
                                await _context.SaveChangesAsync();
                            }

                            // AssetLocation
                            var location = await _context.AssetLocations
                                .FirstOrDefaultAsync(l => l.Name == assetLocationText);
                            if (location == null)
                            {
                                location = new AssetLocation
                                {
                                    Name = string.IsNullOrEmpty(assetLocationText) ? "Unknown" : assetLocationText
                                };
                                _context.AssetLocations.Add(location);
                                await _context.SaveChangesAsync();
                            }

                            // Department
                            var department = await _context.Departments
                                .FirstOrDefaultAsync(d => d.DepartmentName == departmentText);
                            if (department == null)
                            {
                                department = new Department
                                {
                                    DepartmentName = string.IsNullOrEmpty(departmentText) ? "Unknown" : departmentText
                                };
                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }

                            // ===== Convert dates =====
                            DateTime? grnDate = SafeExcelDate(worksheet.Cells[row, 12]);
                            DateTime? invoiceDate = SafeExcelDate(worksheet.Cells[row, 14]);
                            DateTime? endDate = SafeExcelDate(worksheet.Cells[row, 15]);

                            // ===== Normalize Status =====
                            string status = string.IsNullOrEmpty(statusText) ? "WORKING" : statusText.ToUpper();

                            var printer = new Printer
                            {
                                ITAssetTag = itAssetTag,
                                PrinterMake = printerMake,
                                PrinterModel = printerModel,
                                SerialNumber = serialNumber,
                                CartridgeType = cartridgeType,
                                PrinterTypeId = printerType.PrinterTypeId,
                                AssetLocationId = location.AssetLocationId,
                                DepartmentId = department.DepartmentId,
                                Division = division,
                                Status = status,
                                Warranty = warranty,
                                GRNDate = grnDate,
                                GRNNumber = grnNumber,
                                InvoiceDate = invoiceDate,
                                EndDate = endDate
                            };

                            printers.Add(printer);
                        }
                    }
                }

                // ===== Remove existing printers if overwriteExisting is true =====
                if (overwriteExisting)
                {
                    _context.Printers.RemoveRange(_context.Printers);
                    await _context.SaveChangesAsync();
                }

                // ===== Add imported printers =====
                if (printers.Any())
                {
                    await _context.Printers.AddRangeAsync(printers);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Successfully imported {printers.Count} printers.";
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
