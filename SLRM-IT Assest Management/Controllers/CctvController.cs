using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;
using System.Diagnostics;
using System.Globalization;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class CCTVController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CCTVController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CCTV
        public async Task<IActionResult> Index(int page = 1, int pageSize = 15)
        {
            var cctvsQuery = _context.Cctv
                .Include(a => a.Department)
                .Include(a => a.AssetLocation);
                //.OrderByDescending(a => a.CctvId);

            var totalCCTVs = await cctvsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCCTVs / (double)pageSize);

            var cctvs = await cctvsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View(cctvs);
        }

        // GET: CCTV/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: CCTV/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cctv cctv)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(cctv);
            }

            _context.Cctv.Add(cctv);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "CCTV/IT Asset added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: CCTV/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cctv = await _context.Cctv.FindAsync(id);
            if (cctv == null)
                return NotFound();

            await PopulateDropdownsAsync();
            return View(cctv);
        }

        // POST: CCTV/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cctv cctv)
        {
            if (id != cctv.CctvId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(cctv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync();
            return View(cctv);
        }

        // GET: CCTV/Delete/5
        // ✅ GET: Shows confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.Cctv
                .Include(a => a.Department)
                .Include(a => a.AssetLocation)
                .FirstOrDefaultAsync(a => a.CctvId == id);

            if (asset == null)
                return NotFound();

            return View(asset);  // ✅ Passes model to Delete.cshtml
        }

        // ✅ POST: Actually deletes (ActionName="Delete" matches form)
        [HttpPost, ActionName("Delete")]  // ← This creates /CCTV/Delete POST endpoint
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Cctv.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            _context.Cctv.Remove(asset);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "CCTV asset deleted successfully!";
            return RedirectToAction(nameof(Index));
        }


        // GET: CCTV/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: CCTV/Import
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

            var importResults = new List<string>();
            var cctvList = new List<Cctv>();

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

                        // Skip header row and process data rows
                        for (int row = 2; row <= rowCount; row++)
                        {
                            // ✅ FIXED: Skip empty rows
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1]?.Text?.Trim()))
                                continue;

                            // ===== MAP EXACTLY TO YOUR EXCEL COLUMNS =====
                            string itAssetTag = $"CCTV-{row - 1:D3}"; // Generate: CCTV-001, CCTV-002
                            string deviceType = worksheet.Cells[row, 2]?.Text?.Trim(); // Device Type (Col B)
                            string model = worksheet.Cells[row, 3]?.Text?.Trim();        // MODEL (Col C)
                            string cameraSerialNumber = worksheet.Cells[row, 4]?.Text?.Trim(); // CAMERA SERIAL NUMBER (Col D)
                            string hardDiskCapacity = worksheet.Cells[row, 5]?.Text?.Trim();   // HARD DISK CAPACITY (Col E)
                            string channel = worksheet.Cells[row, 6]?.Text?.Trim();            // Channel (Col F)
                            int? numberOfHardDisks = int.TryParse(worksheet.Cells[row, 7]?.Text?.Trim(), out var num) ? num : null; // NUMBER OF HARD DISK (Col G)
                            string departmentText = worksheet.Cells[row, 8]?.Text?.Trim();     // DEPARTMENT (Col H)
                            string locationText = worksheet.Cells[row, 9]?.Text?.Trim();       // LOCATION (Col I)
                            string handoverDateText = worksheet.Cells[row, 10]?.Text?.Trim();  // Handover Date (Col J)
                            string statusText = worksheet.Cells[row, 11]?.Text?.Trim();        // Status (Col K)
                            string invoiceDateText = worksheet.Cells[row, 12]?.Text?.Trim();   // Invoice date (Col L)
                            string grnDateText = worksheet.Cells[row, 13]?.Text?.Trim();       // GRN Date (Col M)
                            string grnNumber = worksheet.Cells[row, 14]?.Text?.Trim();         // GRN number (Col N)
                            string warrantyText = worksheet.Cells[row, 15]?.Text?.Trim();      // warranty (Col O)
                            string endDateText = worksheet.Cells[row, 16]?.Text?.Trim();       // End Date (Col P)
                            string scrapDateText = worksheet.Cells[row, 17]?.Text?.Trim();     // Scrap Date (Col Q)
                            string remark = worksheet.Cells[row, 18]?.Text?.Trim();            // Remark (Col R)

                            try
                            {
                                // ===== CREATE/GET Department =====
                                Department department = null;
                                if (!string.IsNullOrEmpty(departmentText))
                                {
                                    department = await _context.Departments
                                        .FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == departmentText.ToLower());
                                    if (department == null)
                                    {
                                        department = new Department { DepartmentName = departmentText };
                                        _context.Departments.Add(department);
                                        await _context.SaveChangesAsync();
                                    }
                                }

                                // ===== CREATE/GET Location =====
                                AssetLocation location = null;
                                if (!string.IsNullOrEmpty(locationText))
                                {
                                    location = await _context.AssetLocations
                                        .FirstOrDefaultAsync(l => l.Name.ToLower() == locationText.ToLower());
                                    if (location == null)
                                    {
                                        location = new AssetLocation { Name = locationText };
                                        _context.AssetLocations.Add(location);
                                        await _context.SaveChangesAsync();
                                    }
                                }

                                // ===== PARSE DATES =====
                                DateTime? handoverDate = ParseExcelDate(handoverDateText);
                                DateTime? invoiceDate = ParseExcelDate(invoiceDateText);
                                DateTime? grnDate = ParseExcelDate(grnDateText);
                                DateTime? endDate = ParseExcelDate(endDateText);
                                DateTime? scrapDate = ParseExcelDate(scrapDateText);

                                // ===== NORMALIZE STATUS =====
                                string status = !string.IsNullOrEmpty(statusText)
                                    ? statusText.ToUpper().Trim() == "ACTIVE" ? "Active" : statusText.Trim()
                                    : "Active";

                                int? warrantyMonths = int.TryParse(warrantyText, out var w) ? w : null;

                                // ===== CREATE CCTV OBJECT =====
                                var cctv = new Cctv
                                {
                                    ITAssetTag = itAssetTag,
                                    DeviceType = deviceType,
                                    Model = model,
                                    CameraSerialNumber = cameraSerialNumber,
                                    HardDiskCapacity = hardDiskCapacity,
                                    Channel = channel,
                                    NumberOfHardDisks = numberOfHardDisks,
                                    DepartmentId = department?.DepartmentId,
                                    AssetLocationId = location?.AssetLocationId,
                                    HandoverDate = handoverDate,
                                    Status = status,
                                    InvoiceDate = invoiceDate,
                                    GRNDate = grnDate,
                                    GRNNumber = grnNumber,
                                    Warranty = warrantyText,
                                    EndDate = endDate,
                                    ScrapDate = scrapDate,
                                    Remark = remark
                                };

                                cctvList.Add(cctv);
                                importResults.Add($"✅ Row {row}: {itAssetTag} - {deviceType} imported successfully");
                            }
                            catch (Exception rowEx)
                            {
                                importResults.Add($"❌ Row {row}: {rowEx.Message}");
                            }
                        }
                    }
                }

                // ===== SAVE BATCH =====
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (overwriteExisting)
                    {
                        _context.Cctv.RemoveRange(_context.Cctv);
                        await _context.SaveChangesAsync();
                    }

                    if (cctvList.Any())
                    {
                        await _context.Cctv.AddRangeAsync(cctvList);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"✅ Successfully imported {cctvList.Count} CCTV assets!";
                    TempData["ImportResults"] = importResults;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Import failed: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Import failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        // ===== HELPER METHOD FOR EXCEL DATE PARSING =====
        private DateTime? ParseExcelDate(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText)) return null;

            // Try multiple date formats from your Excel
            var formats = new[] {
        "dd/MMM/yy", "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-dd",
        "d/MMM/yy", "MM/dd/yyyy", "dd/MM/yy"
    };

            if (DateTime.TryParseExact(dateText.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            return null;
        }

        // Helper: Populate dropdowns
        private async Task PopulateDropdownsAsync()
        {
            ViewData["Departments"] = await _context.Departments.ToListAsync();
            ViewData["AssetLocations"] = await _context.AssetLocations.ToListAsync();
        }
    }
}
