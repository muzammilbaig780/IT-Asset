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
                .Include(a => a.Block)
                .Include(a => a.Department)
                .Include(a => a.Division)
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

                            // NOTE: Column indexes updated to match the provided Excel:
                            // 1=SLNO,2=TYPE,3=DEPARTMENT,4=EMP ID,5=USER NAME,6=HOST NAME,
                            // 7=BLOCK,8=ASSET LOCATION,9=ASSET ID,10=MAKE,11=MODEL,12=SERIAL NO,
                            // 13=PROCESSOR,14=RAM,15=HDD,16=DIVISION,17=ANTIVIRUS,18=STATUS,
                            // 19=OS VERSION,20=AUTOCAD,21=OFFICE,22=WINDOW LICENSE KEY,23=IP ADDRESS,
                            // 24=NITRO,25=AUDIT STATUS

                            string type = worksheet.Cells[row, 2]?.Text?.Trim();
                            string departmentText = worksheet.Cells[row, 3]?.Text?.Trim();
                            string empId = worksheet.Cells[row, 4]?.Text?.Trim();
                            string userName = worksheet.Cells[row, 5]?.Text?.Trim();
                            string hostName = worksheet.Cells[row, 6]?.Text?.Trim();
                            string blockText = worksheet.Cells[row, 7]?.Text?.Trim();
                            string locationText = worksheet.Cells[row, 8]?.Text?.Trim();
                            string assetIdText = worksheet.Cells[row, 9]?.Text?.Trim();
                            string makeText = worksheet.Cells[row, 10]?.Text?.Trim();
                            string modelText = worksheet.Cells[row, 11]?.Text?.Trim();
                            string serialText = worksheet.Cells[row, 12]?.Text?.Trim();
                            string processorText = worksheet.Cells[row, 13]?.Text?.Trim();
                            string ramText = worksheet.Cells[row, 14]?.Text?.Trim();
                            string hddText = worksheet.Cells[row, 15]?.Text?.Trim();
                            string divisionText = worksheet.Cells[row, 16]?.Text?.Trim();
                            string antivirusText = worksheet.Cells[row, 17]?.Text?.Trim();
                            string statusText = worksheet.Cells[row, 18]?.Text?.Trim();
                            string osVersionText = worksheet.Cells[row, 19]?.Text?.Trim();
                            string autoCadText = worksheet.Cells[row, 20]?.Text?.Trim();
                            string officeText = worksheet.Cells[row, 21]?.Text?.Trim();
                            string windowKeyText = worksheet.Cells[row, 22]?.Text?.Trim();
                            string ipText = worksheet.Cells[row, 23]?.Text?.Trim();
                            string nitroText = worksheet.Cells[row, 24]?.Text?.Trim();
                            string auditText = worksheet.Cells[row, 25]?.Text?.Trim();

                            var asset = new Asset
                            {
                                SlNo = int.TryParse(worksheet.Cells[row, 1]?.Text, out int slNo) ? slNo : 0,
                                EmpCode = empId,
                                UserName = userName,
                                HostName = hostName,
                                AssetTag = assetIdText,
                                Make = makeText,
                                Model = modelText,
                                MoniterMake = "NA",
                                MoniterModel = "NA",
                                // default SerialNo to "NA" if empty
                                SerialNo = string.IsNullOrWhiteSpace(serialText) ? "NA" : serialText,
                                Processor = processorText,
                                Ram = ramText,
                                Hdd = hddText,
                                AntiVirus = antivirusText,
                                OSVersion = osVersionText,
                                AutoCad = autoCadText,
                                Office = officeText,
                                WindowLicenseKey = windowKeyText,
                                IPAddress = ipText,
                                Nitro = nitroText,
                                AuditStatus = auditText
                            };

                            // STATUS mapping (col 18)
                            if (!string.IsNullOrEmpty(statusText))
                            {
                                var status = _context.AssetStatuses.FirstOrDefault(s => s.Name == statusText);
                                asset.StatusId = status?.StatusId
                                    ?? _context.AssetStatuses.FirstOrDefault(s => s.Name == "Active")?.StatusId
                                    ?? _context.AssetStatuses.FirstOrDefault()?.StatusId;
                            }
                            else
                            {
                                asset.StatusId = _context.AssetStatuses.FirstOrDefault(s => s.Name == "Active")?.StatusId
                                    ?? _context.AssetStatuses.FirstOrDefault()?.StatusId;
                            }

                            // AssetType mapping (col 2)
                            if (!string.IsNullOrEmpty(type))
                            {
                                var assetType = _context.AssetTypes.FirstOrDefault(t => t.Name == type);
                                if (assetType != null)
                                    asset.AssetTypeId = assetType.AssetTypeId;
                            }

                            // COMPANY handling: keep your current approach (col 2 used previously)
                            string companyText = worksheet.Cells[row, 2]?.Text?.Trim();
                            if (!string.IsNullOrEmpty(companyText))
                            {
                                var company = _context.Companies.FirstOrDefault(c => c.CompanyName == companyText);
                                asset.CompanyId = company?.CompanyId ?? _context.Companies.FirstOrDefault()?.CompanyId;
                            }
                            else
                            {
                                asset.CompanyId = _context.Companies.FirstOrDefault()?.CompanyId;
                            }

                            // ASSET LOCATION mapping (col 8) - try to match by name first
                            if (!string.IsNullOrEmpty(locationText))
                            {
                                var loc = _context.AssetLocations.FirstOrDefault(l => l.Name == locationText);
                                asset.AssetLocationId = loc?.AssetLocationId ?? _context.AssetLocations.FirstOrDefault()?.AssetLocationId;
                            }
                            else
                            {
                                asset.AssetLocationId = _context.AssetLocations.FirstOrDefault()?.AssetLocationId;
                            }

                            // Department mapping (col 3)
                            if (!string.IsNullOrEmpty(departmentText))
                            {
                                var dept = _context.Departments.FirstOrDefault(d => d.DepartmentName == departmentText);
                                asset.DepartmentId = dept?.DepartmentId ?? _context.Departments.FirstOrDefault()?.DepartmentId;
                            }
                            else
                            {
                                asset.DepartmentId = _context.Departments.FirstOrDefault()?.DepartmentId;
                            }

                            // Block mapping (col 7)
                            if (!string.IsNullOrEmpty(blockText))
                            {
                                var block = _context.Blocks.FirstOrDefault(b => b.BlockName == blockText);
                                asset.BlockId = block?.BlockId ?? _context.Blocks.FirstOrDefault()?.BlockId;
                            }
                            else
                            {
                                asset.BlockId = _context.Blocks.FirstOrDefault()?.BlockId;
                            }

                            // Division mapping (col 16)
                            if (!string.IsNullOrEmpty(divisionText))
                            {
                                // note: your Division model property name in migration is DivisionName; adjust matching as needed
                                var division = _context.Divisions.FirstOrDefault(b => b.DivisionName == blockText);
                                asset.DivisionId = division?.DivisionId ?? _context.Divisions.FirstOrDefault()?.DivisionId;
                            }
                            else
                            {
                                asset.DivisionId = _context.Divisions.FirstOrDefault()?.DivisionId;
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


        public async Task<IActionResult> Laptops()
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

            ViewBag.SelectedType = "Laptop";
            return View("Index", laptops); // reuse same Index view
        }

        public async Task<IActionResult> Desktops()
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

            ViewBag.SelectedType = "Desktop";
            return View("Index", desktops); // reuse same Index view
        }


    }
}