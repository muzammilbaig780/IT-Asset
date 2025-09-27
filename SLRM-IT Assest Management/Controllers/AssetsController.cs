using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;
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
            var assets = await _context.Assets.ToListAsync();
            return View(assets);
        }
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetId,SlNo,Type,Department,UserName,EmpCode,HostName,Block,AssetLocation,MoniterMake,MoniterAssetTag,Make,Model,SerialNo,Processor,Ram,Hdd,Division,AntiVirus,Status,OSVersion,AutoCad,Office,WindowLicenseKey,IPAddress,Nitro,AuditStatus")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(asset);
        }
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
            // Use new static property for EPPlus 8+
            OfficeOpenXml.ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid file.";
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

                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var type = worksheet.Cells[row, 2]?.Text?.Trim();
                            if (string.IsNullOrWhiteSpace(type)) continue;

                            var asset = new Asset
                            {
                                SlNo = int.TryParse(worksheet.Cells[row, 1]?.Text, out int slNo) ? slNo : 0,
                                Type = worksheet.Cells[row, 2]?.Text?.Trim(),
                                Department = worksheet.Cells[row, 3]?.Text?.Trim(),

                                UserName = worksheet.Cells[row, 4]?.Text?.Trim(), // Asset Tag
                                EmpCode = worksheet.Cells[row, 5]?.Text?.Trim(),
                                HostName = worksheet.Cells[row, 6]?.Text?.Trim(),
                                Block = worksheet.Cells[row, 7]?.Text?.Trim(),
                                AssetLocation = worksheet.Cells[row, 8]?.Text?.Trim(),
                                AssetTag = worksheet.Cells[row, 9]?.Text?.Trim(),
                                Make = worksheet.Cells[row, 10]?.Text?.Trim(),
                                Model = worksheet.Cells[row, 11]?.Text?.Trim(),
                                MoniterMake = worksheet.Cells[row,12]?.Text?.Trim(),
                                MoniterModel = worksheet.Cells[row,13]?.Text?.Trim(),
                                SerialNo = worksheet.Cells[row, 14]?.Text?.Trim(),
                                Processor = worksheet.Cells[row, 15]?.Text?.Trim(),

                                Ram = worksheet.Cells[row, 16]?.Text?.Trim(),
                                Hdd = worksheet.Cells[row, 17]?.Text?.Trim(),
                                Division = worksheet.Cells[row, 18]?.Text?.Trim(),
                                AntiVirus = worksheet.Cells[row, 19]?.Text?.Trim(),
                                Status = worksheet.Cells[row, 20]?.Text?.Trim(),

                                OSVersion = worksheet.Cells[row, 21]?.Text?.Trim(),
                                AutoCad = worksheet.Cells[row, 22]?.Text?.Trim(),
                                Office = worksheet.Cells[row, 23]?.Text?.Trim(),
                                WindowLicenseKey = worksheet.Cells[row, 24]?.Text?.Trim(),
                                IPAddress = worksheet.Cells[row, 25]?.Text?.Trim(),
                                Nitro = worksheet.Cells[row, 26]?.Text?.Trim(),
                                AuditStatus = worksheet.Cells[row, 27]?.Text?.Trim()
                            };

                            assets.Add(asset);

                        }
                    }
                }

                if (overwriteExisting)
                    _context.Assets.RemoveRange(_context.Assets);

                if (assets.Any())
                {
                    await _context.Assets.AddRangeAsync(assets);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Imported {assets.Count} assets successfully.";
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