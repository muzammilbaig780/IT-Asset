using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SLRM_IT_Assest_Management.Models;


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

                // ✅ Auto-calculate expiry date
                if (asset.InvoiceDate.HasValue && asset.Warranty.HasValue)
                {
                    asset.ExpiryDate = asset.InvoiceDate.Value.AddMonths(asset.Warranty.Value);
                }



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

            var asset = await _context.Assets
                .Include(a => a.AssetType)
                .Include(a => a.Company)
                .Include(a => a.AssetStatus)
                .Include(a => a.AssetLocation)
                .Include(a => a.Department)
                .Include(a => a.Block)
                .Include(a => a.Division)
                .Include(a => a.TransferLogs)
                .FirstOrDefaultAsync(a => a.AssetId == id);

            if (asset == null)
            {
                return NotFound();
            }

            // Initialize ViewBag with lists (no need for ?? as ToListAsync will never return null)
            ViewBag.AssetTypes = await _context.AssetTypes.ToListAsync();
            ViewBag.Companies = await _context.Companies.ToListAsync();
            ViewBag.AssetStatuses = await _context.AssetStatuses.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Blocks = await _context.Blocks.ToListAsync();
            ViewBag.Divisions = await _context.Divisions.ToListAsync();

            return View(asset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
     int id,
     Asset asset,
        string? transferReason,
        string? transferredBy,
        string? remarks,
        string? toUserName,
        string? toEmpCode,
        string? toDepartment,
        string? toLocation,
     bool isTransfer)
        {
            if (id != asset.AssetId)
                return NotFound();

            // ✅ Remove navigation validation
            ModelState.Remove("AssetType");
            ModelState.Remove("Company");
            ModelState.Remove("AssetStatus");
            ModelState.Remove("Department");
            ModelState.Remove("AssetLocation");
            ModelState.Remove("Block");
            ModelState.Remove("Division");
            ModelState.Remove("TransferLogs");
            ModelState.Remove("Warranty");
            ModelState.Remove("GRNDate");
            ModelState.Remove("InvoiceDate");
            ModelState.Remove("ExpiryDate");

            var errors = ModelState
    .Where(x => x.Value.Errors.Count > 0)
    .Select(x => new {
        Field = x.Key,
        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
    }).ToList();



            if (!ModelState.IsValid)
            {
                await LoadViewBags();
                return View(asset);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingAsset = await _context.Assets
                    .FirstOrDefaultAsync(a => a.AssetId == id);

                if (existingAsset == null)
                    return NotFound();

                // ✅ Store OLD values safely
                var oldUserName = existingAsset.UserName;
                var oldEmpCode = existingAsset.EmpCode;
                var oldDepartmentId = existingAsset.DepartmentId;
                var oldLocationId = existingAsset.AssetLocationId;

                // ✅ Apply new values ONLY if transfer is checked
                if (isTransfer && !string.IsNullOrWhiteSpace(toUserName))
                {
                    existingAsset.UserName = toUserName;
                    existingAsset.EmpCode = toEmpCode;

                    if (!string.IsNullOrWhiteSpace(toDepartment))
                    {
                        var dept = await _context.Departments
                            .FirstOrDefaultAsync(x => x.DepartmentName == toDepartment);
                        if (dept != null)
                            existingAsset.DepartmentId = dept.DepartmentId;
                    }

                    if (!string.IsNullOrWhiteSpace(toLocation))
                    {
                        var loc = await _context.AssetLocations
                            .FirstOrDefaultAsync(x => x.Name == toLocation);
                        if (loc != null)
                            existingAsset.AssetLocationId = loc.AssetLocationId;
                    }
                }

                // ✅ Detect real transfer
                bool isActualTransfer =
    (oldUserName != existingAsset.UserName ||
     oldEmpCode != existingAsset.EmpCode ||
     oldDepartmentId != existingAsset.DepartmentId ||
     oldLocationId != existingAsset.AssetLocationId);

                if (isTransfer && isActualTransfer)
                {
                    var log = new AssetTransferLog
                    {
                        AssetId = existingAsset.AssetId,
                        FromUserName = oldUserName,
                        FromEmpCode = oldEmpCode,
                        FromDepartmentId = oldDepartmentId,
                        ToUserName = existingAsset.UserName,
                        ToEmpCode = existingAsset.EmpCode,
                        ToDepartmentId = existingAsset.DepartmentId,
                        TransferReason = transferReason ?? "Not specified",
                        TransferredBy = transferredBy ?? User.Identity?.Name ?? "System",
                        Remarks = remarks,
                        TransferDate = DateTime.Now
                    };

                    _context.AssetTransferLogs.Add(log);
                    TempData["SuccessMessage"] = "Asset transferred successfully!";
                }

                else
                {
                    TempData["SuccessMessage"] = "Asset updated successfully!";
                }

                // ✅ Update remaining asset fields (SAFE)
                existingAsset.Make = asset.Make;
                existingAsset.Model = asset.Model;
                existingAsset.SerialNo = asset.SerialNo;
                existingAsset.Processor = asset.Processor;
                existingAsset.Ram = asset.Ram;
                existingAsset.Hdd = asset.Hdd;
                existingAsset.OSVersion = asset.OSVersion;
                existingAsset.Office = asset.Office;
                existingAsset.AntiVirus = asset.AntiVirus;
                existingAsset.WindowLicenseKey = asset.WindowLicenseKey;
                existingAsset.IPAddress = asset.IPAddress;
                existingAsset.Nitro = asset.Nitro;
                existingAsset.AuditStatus = asset.AuditStatus;
                existingAsset.GRNNumber = asset.GRNNumber;
                existingAsset.ExpiryDate = asset.ExpiryDate;

                existingAsset.IsTransferred = true;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Update failed: " + ex.Message);
                await LoadViewBags();
                return View(asset);
            }
        }
        [HttpGet]
        public async Task<IActionResult> TransferHistory(int? id)
        {
            if (id == null)
                return BadRequest("Asset ID is required");

            var asset = await _context.Assets
                .Include(a => a.AssetType)
                .Include(a => a.Department)
                .Include(a => a.AssetStatus)
                .FirstOrDefaultAsync(a => a.AssetId == id);

            if (asset == null)
                return NotFound("Asset not found");

            var history = await _context.AssetTransferLogs
                .Include(x => x.FromDepartment)
                .Include(x => x.ToDepartment)
                .Where(x => x.AssetId == id)
                .OrderByDescending(x => x.TransferDate)
                .ToListAsync();

            ViewBag.Asset = asset;
            return View(history);
        }


        private async Task LoadViewBags()
        {
            ViewBag.AssetTypes = await _context.AssetTypes.ToListAsync();
            ViewBag.Companies = await _context.Companies.ToListAsync();
            ViewBag.AssetStatuses = await _context.AssetStatuses.ToListAsync();
            ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Blocks = await _context.Blocks.ToListAsync();
            ViewBag.Divisions = await _context.Divisions.ToListAsync();
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

                            // ===== Read Common Columns =====
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

                            // Detect Laptop
                            bool isLaptop = assetTypeText.Equals("Laptop", StringComparison.OrdinalIgnoreCase);

                            // ===== Conditional column mapping =====
                            string monitorMakeText = isLaptop ? "NA" : worksheet.Cells[row, 12]?.Text?.Trim();
                            string monitorModelText = isLaptop ? "NA" : worksheet.Cells[row, 13]?.Text?.Trim();
                            string cpuSerialNoModelText = isLaptop ? "NA" : worksheet.Cells[row, 14]?.Text?.Trim();
                            string serialText = isLaptop
                                ? worksheet.Cells[row, 12]?.Text?.Trim()   // For laptops, serial is in col 12
                                : worksheet.Cells[row, 14]?.Text?.Trim();  // For desktops, serial is in col 14

                            // Continue reading other columns (shifted)
                            int nextCol = isLaptop ? 13 : 15;

                            string cpuSerialNo = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string processorText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string ramText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string hddText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string divisionText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string antivirusText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string statusText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string osVersionText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string autoCadText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string officeText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string windowKeyText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string ipText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string nitroText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string auditText = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string catridgeType = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string gRNNumbertext = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string gRNDatetext = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string invoiceDate = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string warranty = worksheet.Cells[row, nextCol++]?.Text?.Trim();
                            string expiryDate = worksheet.Cells[row, nextCol++]?.Text?.Trim();

                            // ===== Ensure master data exists =====
                            var assetType = await _context.AssetTypes.FirstOrDefaultAsync(a => a.Name == assetTypeText);
                            if (assetType == null && !string.IsNullOrEmpty(assetTypeText))
                            {
                                assetType = new AssetType { Name = assetTypeText };
                                _context.AssetTypes.Add(assetType);
                                await _context.SaveChangesAsync();
                            }

                            var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == departmentText);
                            if (department == null && !string.IsNullOrEmpty(departmentText))
                            {
                                department = new Department { DepartmentName = departmentText };
                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }

                            var block = await _context.Blocks.FirstOrDefaultAsync(b => b.BlockName == blockText);
                            if (block == null && !string.IsNullOrEmpty(blockText))
                            {
                                block = new Block { BlockName = blockText };
                                _context.Blocks.Add(block);
                                await _context.SaveChangesAsync();
                            }

                            var location = await _context.AssetLocations.FirstOrDefaultAsync(l => l.Name == locationText);
                            if (location == null && !string.IsNullOrEmpty(locationText))
                            {
                                location = new AssetLocation { Name = locationText };
                                _context.AssetLocations.Add(location);
                                await _context.SaveChangesAsync();
                            }

                            var division = await _context.Divisions.FirstOrDefaultAsync(d => d.DivisionName == divisionText);
                            if (division == null && !string.IsNullOrEmpty(divisionText))
                            {
                                division = new Division { DivisionName = divisionText };
                                _context.Divisions.Add(division);
                                await _context.SaveChangesAsync();
                            }

                            // Default company = SLR Metaliks
                            string companyText = "SLR Metaliks";
                            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyName == companyText);
                            if (company == null)
                            {
                                company = new Company { CompanyName = companyText };
                                _context.Companies.Add(company);
                                await _context.SaveChangesAsync();
                            }

                            // ===== Status (SAFE FK HANDLING) =====
                            int statusId = 0;

                            if (!string.IsNullOrWhiteSpace(statusText))
                            {
                                string normalized = statusText.Trim().ToUpper();

                                var status = await _context.AssetStatuses
                                    .FirstOrDefaultAsync(s => s.Name.ToUpper() == normalized);

                                if (status == null)
                                {
                                    // ✅ AUTO INSERT if status does not exist
                                    status = new Status
                                    {
                                        Name = normalized
                                    };

                                    _context.AssetStatuses.Add(status);
                                    await _context.SaveChangesAsync();
                                }

                                statusId = status.StatusId;
                            }
                            else
                            {
                                // ✅ SAFE DEFAULT STATUS (must exist in DB)
                                var defaultStatus = await _context.AssetStatuses.FirstOrDefaultAsync();
                                statusId = defaultStatus?.StatusId ?? 1;
                            }


                            // --- Convert GRN Date ---
                            // --- Convert GRN Date ---
                            DateOnly? grnDate = null;

                            if (!string.IsNullOrWhiteSpace(gRNDatetext) &&
                                gRNDatetext.Trim().ToUpper() != "NA")
                            {
                                if (DateOnly.TryParse(gRNDatetext, out var parsed))
                                {
                                    grnDate = parsed;
                                }
                                else if (double.TryParse(gRNDatetext, out var oa))
                                {
                                    grnDate = DateOnly.FromDateTime(DateTime.FromOADate(oa));
                                }
                            }

                            // --- Convert Invoice Date ---
                            DateOnly? invoiceDateParsed = null;

                            if (!string.IsNullOrWhiteSpace(invoiceDate) &&
                                invoiceDate.Trim().ToUpper() != "NA")
                            {
                                // Try parse as normal date
                                if (DateOnly.TryParse(invoiceDate, out var parsedInvoice))
                                    invoiceDateParsed = parsedInvoice;
                                // Try parse as Excel numeric date
                                else if (double.TryParse(invoiceDate, out var oaInvoice))
                                    invoiceDateParsed = DateOnly.FromDateTime(DateTime.FromOADate(oaInvoice));
                            }


                            // --- Convert Warranty (months) ---
                            int? warrantyMonths = null;

                            if (int.TryParse(warranty, out var parsedWarranty))
                                warrantyMonths = parsedWarranty;

                            // 🚀 FIX: Prevent NULL value going to SQL
                            if (warrantyMonths == null)
                            {
                                warrantyMonths = 0; // or 12 (set default you prefer)
                            }


                            DateOnly? expiryDateParsed = null;

                            // If Excel has a real date (not NA)
                            if (!string.IsNullOrWhiteSpace(expiryDate) &&
                                expiryDate.Trim().ToUpper() != "NA")
                            {
                                if (DateOnly.TryParse(expiryDate, out var parsedExpiry))
                                    expiryDateParsed = parsedExpiry;
                                else if (double.TryParse(expiryDate, out var oaExpiry))
                                    expiryDateParsed = DateOnly.FromDateTime(DateTime.FromOADate(oaExpiry));
                            }

                            // If Excel expiry is empty (not NA), then compute using invoice date + warranty
                            if (expiryDateParsed == null &&
                                string.IsNullOrWhiteSpace(expiryDate))  // Only if no NA
                            {
                                if (invoiceDateParsed != null && warrantyMonths != null)
                                    expiryDateParsed = invoiceDateParsed.Value.AddMonths(warrantyMonths.Value);
                            }


                            // ===== Build Asset =====
                            var asset = new Asset
                            {
                                SlNo = int.TryParse(worksheet.Cells[row, 1]?.Text, out int slNo) ? slNo : 0,
                                EmpCode = empId,
                                UserName = userName,
                                HostName = hostName,
                                AssetTag = assetIdText,
                                Make = makeText,
                                Model = modelText,
                                MoniterMake = monitorMakeText,
                                MoniterModel = monitorModelText,
                                SerialNo = string.IsNullOrWhiteSpace(serialText) ? "NA" : serialText,
                                CPUSerialNo = string.IsNullOrWhiteSpace(cpuSerialNo) ? "NA" : cpuSerialNo,
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
                                CatridgeType = catridgeType,
                                GRNNumber = gRNNumbertext,
                                GRNDate = grnDate,
                                InvoiceDate = invoiceDateParsed,
                                Warranty = warrantyMonths,
                                ExpiryDate = expiryDateParsed,
                                AssetTypeId = assetType?.AssetTypeId ?? 1,
                                DepartmentId = department?.DepartmentId,
                                BlockId = block?.BlockId,
                                AssetLocationId = location?.AssetLocationId,
                                DivisionId = division?.DivisionId,
                                CompanyId = company.CompanyId,
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