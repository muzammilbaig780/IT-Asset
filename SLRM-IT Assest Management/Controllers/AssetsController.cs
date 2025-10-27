using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;
using SLRM_IT_Assest_Management.ViewModels;
using System.Globalization;

namespace AssetManagement.Controllers
{
    public class AssetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AssetsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Assets
        public async Task<IActionResult> Index()
        {
            var assets = await _context.Assets
                .Include(a => a.AssetType)
                .Include(a => a.AssetLocation)
                .Include(a => a.AssetStatus)
                .Include(a => a.Company)
                .ToListAsync();

            return View(assets);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AssetTypes = await _context.AssetTypes.ToListAsync();
            ViewBag.AssetStatuses = await _context.AssetStatuses.ToListAsync();
            ViewBag.Companies = await _context.Companies.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asset asset)
        {
            if (ModelState.IsValid)
            {
                asset.SlNo = (_context.Assets.Max(a => (int?)a.SlNo) ?? 0) + 1;
                _context.Add(asset);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asset added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate dropdowns
            ViewBag.AssetTypes = _context.AssetTypes.ToList();
            ViewBag.AssetStatuses = _context.AssetStatuses.ToList();
            ViewBag.AssetLocations = _context.AssetLocations.ToList();
            ViewBag.Companies = _context.Companies.ToList();

            return View(asset);
        }
        //private async Task<int> GetNextSlNo()
        //{
        //    var maxSlNo = await _context.Assets.MaxAsync(a => (int?)a.SlNo);
        //    return (maxSlNo ?? 0) + 1;
        //}

        // GET: Assets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            return View(asset);
        }

        // POST: Assets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("\"AssetId,SlNo,Type,Department,UserName,HostName,Block,AssetLocation,AssetTag,Make,Model,SerialNo,Processor,Ram,Hdd,Division,AntiVirus,Status,OSVersion,AutoCad,Office,WindowLicenseKey,IPAddress,Nitro,AuditStatus")] Asset asset, IFormFile? imageFile)
        {
            if (id != asset.AssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload if a new file is provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        //asset.DeviceImage = "/images/" + fileName;
                    }

                    _context.Update(asset);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Asset updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetExists(asset.AssetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(asset);
        }

        // GET: Assets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asset deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.AssetId == id);
        }

        // GET: Import
        //public IActionResult Import()
        //{
        //    return View();
        //}

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

            var assets = new List<Asset>();

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

                        for (int row = 2; row <= rowCount; row++) // Skip header
                        {
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1]?.Text)) continue;

                            string type = worksheet.Cells[row, 2]?.Text?.Trim();

                            var asset = new Asset
                            {
                                SlNo = int.TryParse(worksheet.Cells[row, 1]?.Text, out int slNo) ? slNo : 0,
                                // TYPE column handled below
                                Department = worksheet.Cells[row, 3]?.Text?.Trim(),
                                EmpCode = worksheet.Cells[row, 4]?.Text?.Trim(),
                                UserName = worksheet.Cells[row, 5]?.Text?.Trim(),
                                HostName = worksheet.Cells[row, 6]?.Text?.Trim(),
                                Block = worksheet.Cells[row, 7]?.Text?.Trim(),
                                // ASSET LOCATION/FLOOR
                                AssetTag = worksheet.Cells[row, 9]?.Text?.Trim(),
                                Make = worksheet.Cells[row, 10]?.Text?.Trim(),
                                Model = worksheet.Cells[row, 11]?.Text?.Trim(),
                                MoniterMake = worksheet.Cells[row, 12]?.Text?.Trim(),
                                MoniterModel = worksheet.Cells[row, 13]?.Text?.Trim(),
                                SerialNo = worksheet.Cells[row, 14]?.Text?.Trim(),
                                Processor = worksheet.Cells[row, 15]?.Text?.Trim(),
                                Ram = worksheet.Cells[row, 16]?.Text?.Trim(),
                                Hdd = worksheet.Cells[row, 17]?.Text?.Trim(),
                                Division = worksheet.Cells[row, 18]?.Text?.Trim(),
                                AntiVirus = worksheet.Cells[row, 20]?.Text?.Trim(),
                                OSVersion = worksheet.Cells[row, 21]?.Text?.Trim(),
                                AutoCad = worksheet.Cells[row, 22]?.Text?.Trim(),
                                Office = worksheet.Cells[row, 23]?.Text?.Trim(),
                                WindowLicenseKey = worksheet.Cells[row, 24]?.Text?.Trim(),
                                IPAddress = worksheet.Cells[row, 25]?.Text?.Trim(),
                                Nitro = worksheet.Cells[row, 26]?.Text?.Trim(),
                                AuditStatus = worksheet.Cells[row, 27]?.Text?.Trim()
                            };

                            // STATUS (column 18) maps to AssetStatus if you have a related entity
                            string statusText = worksheet.Cells[row, 19]?.Text?.Trim();

                            if (!string.IsNullOrEmpty(statusText))
                            {
                                // Try to find the status in the database
                                var status = _context.AssetStatuses.FirstOrDefault(s => s.Name == statusText);

                                // If found, use it — otherwise, assign a default one
                                asset.StatusId = status != null
                                    ? status.StatusId
                                    : _context.AssetStatuses.FirstOrDefault(s => s.Name == "Active")?.StatusId
                                        ?? _context.AssetStatuses.FirstOrDefault()?.StatusId; // final fallback
                            }
                            else
                            {
                                // If Excel cell is empty, assign a default
                                asset.StatusId = _context.AssetStatuses.FirstOrDefault(s => s.Name == "Active")?.StatusId
                                    ?? _context.AssetStatuses.FirstOrDefault()?.StatusId;
                            }

                            // TYPE mapping (AssetType)
                            if (!string.IsNullOrEmpty(type))
                            {
                                var assetType = _context.AssetTypes.FirstOrDefault(t => t.Name == type);
                                if (assetType != null)
                                    asset.AssetTypeId = assetType.AssetTypeId;
                            }

                            // COMPANY handling (adjust column index as per your Excel)
                            string companyText = worksheet.Cells[row, 2]?.Text?.Trim(); // Example column
                            if (!string.IsNullOrEmpty(companyText))
                            {
                                var company = _context.Companies.FirstOrDefault(c => c.CompanyName == companyText);
                                if (company != null)
                                    asset.CompanyId = company.CompanyId;
                                else
                                    asset.CompanyId = _context.Companies.FirstOrDefault()?.CompanyId; // fallback
                            }
                            else
                            {
                                asset.CompanyId = _context.Companies.FirstOrDefault()?.CompanyId; // fallback if blank
                            }




                            // ASSET LOCATION mapping (column 8)
                            string locationText = worksheet.Cells[row, 8]?.Text?.Trim();
                            if (!string.IsNullOrEmpty(locationText))
                            {
                                var defaultLocation = _context.AssetLocations.FirstOrDefault();
                                if (defaultLocation != null)
                                {
                                    asset.AssetLocationId = defaultLocation.AssetLocationId;
                                }
                            }

                            
                            assets.Add(asset);
                        }
                    }
                }

                if (overwriteExisting)
                {
                    _context.Assets.RemoveRange(_context.Assets);
                }

                if (assets.Any())
                {
                    await _context.Assets.AddRangeAsync(assets);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Successfully imported {assets.Count} assets.";
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