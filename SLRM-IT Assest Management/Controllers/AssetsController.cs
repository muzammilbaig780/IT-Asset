using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;
using SLRM_IT_Assest_Management.ViewModels;
using System.Drawing.Printing;
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

        private async Task PopulateDropDownsAsync()
        {
            ViewBag.AssetTypes = await _context.AssetTypes.ToListAsync();
            ViewBag.AssetStatuses = await _context.AssetStatuses.ToListAsync();
            ViewBag.Companies = await _context.Companies.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();
            ViewBag.Blocks = await _context.Blocks.ToListAsync();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Divisions = await _context.Divisions.ToListAsync();
        }

        public async Task<IActionResult> Index(string filter = "All", int page = 1, string pageSize = "25")
        {
            var query = _context.Assets
                .Include(a => a.Company)
                .Include(a => a.AssetType)
                .Include(a => a.AssetLocation)
                .Include(a => a.AssetStatus)
                .Include(a => a.Department)
                .Include(a => a.Division)
                .Include(a => a.Block)
                .AsQueryable();

            if (filter != "All" && !string.IsNullOrEmpty(filter))
            {
                query = query.Where(a => a.AssetType.Name == filter);
            }

            var totalRecords = await query.CountAsync();

            // ✅ Handle “All” case
            int pageSizeValue;
            if (pageSize.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                pageSizeValue = totalRecords;
            }
            else
            {
                pageSizeValue = int.TryParse(pageSize, out var ps) ? ps : 25;
            }

            var assets = await query
                .OrderBy(a => a.AssetId)
                .Skip((page - 1) * pageSizeValue)
                .Take(pageSizeValue)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.Filter = filter;

            return View(assets);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AssetTypes = await _context.AssetTypes.ToListAsync();
            ViewBag.AssetStatuses = await _context.AssetStatuses.ToListAsync();
            ViewBag.Companies = await _context.Companies.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();
            ViewBag.Blocks = await _context.Blocks.ToListAsync();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Divisions = await _context.Divisions.ToListAsync();

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
            ViewBag.Blocks = _context.Blocks.ToList();
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Divisions = _context.Divisions.ToList();

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

        //GET: Import
      

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

                        for (int row = 2; row <= rowCount; row++) // skip header
                        {
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1]?.Text)) continue;

                            // ===== Read Excel columns =====
                            string assetTypeText = worksheet.Cells[row, 2]?.Text?.Trim();
                            string departmentText = worksheet.Cells[row, 3]?.Text?.Trim();
                            string empId = worksheet.Cells[row, 4]?.Text?.Trim();
                            string userName = worksheet.Cells[row, 5]?.Text?.Trim();
                            string hostName = worksheet.Cells[row, 6]?.Text?.Trim();
                            string blockText = worksheet.Cells[row, 7]?.Text?.Trim();
                            string locationText = worksheet.Cells[row, 8]?.Text?.Trim();
                            string assetIdText = worksheet.Cells[row, 9]?.Text?.Trim();
                            string makeText = worksheet.Cells[row, 10]?.Text?.Trim();
                            string modelText = worksheet.Cells[row, 11]?.Text?.Trim();
                            string monitorMakeText = worksheet.Cells[row, 12]?.Text?.Trim();
                            string monitorModelText = worksheet.Cells[row, 13]?.Text?.Trim();
                            string serialText = worksheet.Cells[row, 14]?.Text?.Trim();
                            string processorText = worksheet.Cells[row, 15]?.Text?.Trim();
                            string ramText = worksheet.Cells[row, 16]?.Text?.Trim();
                            string hddText = worksheet.Cells[row, 17]?.Text?.Trim();
                            string divisionText = worksheet.Cells[row, 18]?.Text?.Trim();
                            string antivirusText = worksheet.Cells[row, 19]?.Text?.Trim();
                            string statusText = worksheet.Cells[row, 20]?.Text?.Trim();
                            string osVersionText = worksheet.Cells[row, 21]?.Text?.Trim();
                            string autoCadText = worksheet.Cells[row, 22]?.Text?.Trim();
                            string officeText = worksheet.Cells[row, 23]?.Text?.Trim();
                            string windowKeyText = worksheet.Cells[row, 24]?.Text?.Trim();
                            string ipText = worksheet.Cells[row, 25]?.Text?.Trim();
                            string nitroText = worksheet.Cells[row, 26]?.Text?.Trim();
                            string auditText = worksheet.Cells[row, 27]?.Text?.Trim();

                            // ===== Ensure master data exists =====

                            // 🔹 Asset Type
                            var assetType = await _context.AssetTypes
                                .FirstOrDefaultAsync(a => a.Name == assetTypeText);
                            if (assetType == null && !string.IsNullOrEmpty(assetTypeText))
                            {
                                assetType = new AssetType { Name = assetTypeText };
                                _context.AssetTypes.Add(assetType);
                                await _context.SaveChangesAsync();
                            }

                            // 🔹 Department
                            var department = await _context.Departments
                                .FirstOrDefaultAsync(d => d.DepartmentName == departmentText);
                            if (department == null && !string.IsNullOrEmpty(departmentText))
                            {
                                department = new Department { DepartmentName = departmentText };
                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }

                            // 🔹 Block
                            var block = await _context.Blocks
                                .FirstOrDefaultAsync(b => b.BlockName == blockText);
                            if (block == null && !string.IsNullOrEmpty(blockText))
                            {
                                block = new Block { BlockName = blockText };
                                _context.Blocks.Add(block);
                                await _context.SaveChangesAsync();
                            }

                            // 🔹 Location
                            var location = await _context.AssetLocations
                                .FirstOrDefaultAsync(l => l.Name == locationText);
                            if (location == null && !string.IsNullOrEmpty(locationText))
                            {
                                location = new AssetLocation { Name = locationText };
                                _context.AssetLocations.Add(location);
                                await _context.SaveChangesAsync();
                            }

                            // 🔹 Division
                            var division = await _context.Divisions
                                .FirstOrDefaultAsync(d => d.DivisionName == divisionText);
                            if (division == null && !string.IsNullOrEmpty(divisionText))
                            {
                                division = new Division { DivisionName = divisionText };
                                _context.Divisions.Add(division);
                                await _context.SaveChangesAsync();
                            }

                            // 🔹 Company (Default = SLR Metaliks)
                            string companyText = "SLR Metaliks";
                            var company = await _context.Companies
                                .FirstOrDefaultAsync(c => c.CompanyName == companyText);
                            if (company == null)
                            {
                                company = new Company { CompanyName = companyText };
                                _context.Companies.Add(company);
                                await _context.SaveChangesAsync();
                            }

                            // 🔹 Status
                            int statusId = 1;
                            if (!string.IsNullOrEmpty(statusText))
                            {
                                var existingStatus = await _context.AssetStatuses
                                    .FirstOrDefaultAsync(s => s.Name.ToLower() == statusText.ToLower());
                                if (existingStatus == null)
                                {
                                    var newStatus = new Status { Name = statusText };
                                    _context.AssetStatuses.Add(newStatus);
                                    await _context.SaveChangesAsync();
                                    statusId = newStatus.StatusId;
                                }
                                else
                                {
                                    statusId = existingStatus.StatusId;
                                }
                            }

                            // ===== Build Asset Object =====
                            var asset = new Asset
                            {
                                SlNo = int.TryParse(worksheet.Cells[row, 1]?.Text, out int slNo) ? slNo : 0,
                                EmpCode = empId,
                                UserName = userName,
                                HostName = hostName,
                                AssetTag = assetIdText,
                                Make = makeText,
                                Model = modelText,
                                MoniterMake = string.IsNullOrWhiteSpace(monitorMakeText) ? "NA" : monitorMakeText,
                                MoniterModel = string.IsNullOrWhiteSpace(monitorModelText) ? "NA" : monitorModelText,
                                SerialNo = string.IsNullOrWhiteSpace(serialText) ? "NA" : serialText,
                                Processor = string.IsNullOrWhiteSpace(processorText) ? "NA" : processorText,
                                Ram = string.IsNullOrWhiteSpace(ramText) ? "NA" : ramText,
                                Hdd = string.IsNullOrWhiteSpace(hddText) ? "NA" : hddText,
                                AntiVirus = string.IsNullOrWhiteSpace(antivirusText) ? "NA" : antivirusText,
                                OSVersion = string.IsNullOrWhiteSpace(osVersionText) ? "NA" : osVersionText,
                                AutoCad = string.IsNullOrWhiteSpace(autoCadText) ? "NA" : autoCadText,
                                Office = string.IsNullOrWhiteSpace(officeText) ? "NA" : officeText,
                                WindowLicenseKey = string.IsNullOrWhiteSpace(windowKeyText) ? "NA" : windowKeyText,
                                IPAddress = string.IsNullOrWhiteSpace(ipText) ? "NA" : ipText,
                                Nitro = string.IsNullOrWhiteSpace(nitroText) ? "NA" : nitroText,
                                AuditStatus = string.IsNullOrWhiteSpace(auditText) ? "NA" : auditText,
                                AssetTypeId = assetType?.AssetTypeId ?? 1,
                                DepartmentId = department?.DepartmentId,
                                BlockId = block?.BlockId,
                                AssetLocationId = location?.AssetLocationId,
                                DivisionId = division?.DivisionId,
                                CompanyId = company.CompanyId,   // ✅ FIXED — always set to SLR Metaliks
                                StatusId = statusId
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

                TempData["SuccessMessage"] = $"Successfully imported {assets.Count} assets.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Import failed: " + ex.Message;
                return RedirectToAction(nameof(Import));
            }
        }

        public async Task<IActionResult> Laptops(int pageSize = 10)
        {
            var laptops = await _context.Assets
                .Include(a => a.Company)
                .Include(a => a.AssetType)
                .Include(a => a.AssetLocation)
                .Include(a => a.AssetStatus)
                .Include(a => a.Department)
                .Include(a => a.Division)
                .Include(a => a.Block)
                .Where(a => a.AssetType.Name == "Laptop")
                .ToListAsync();
            ViewBag.PageSize = pageSize;
            ViewBag.Filter = "Laptop"; ;
            return View("Index", laptops); // reuse same Index view
        }

        public async Task<IActionResult> Desktops(int pageSize = 10)
        {
            var desktops = await _context.Assets
                .Include(a => a.Company)
                .Include(a => a.AssetType)
                .Include(a => a.AssetLocation)
                .Include(a => a.AssetStatus)
                .Include(a => a.Department)
                .Include(a => a.Division)
                .Include(a => a.Block)
                .Where(a => a.AssetType.Name == "Desktop")
                .ToListAsync();

            ViewBag.PageSize = pageSize;
            ViewBag.Filter = "Desktop";
            return View("Index", desktops); // reuse same Index view
        }


    }
}