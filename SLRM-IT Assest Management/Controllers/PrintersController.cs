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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // Query to get printers and include related entities like AssetType, AssetLocation, and Department
            var printersQuery = _context.Printers
                .Include(p => p.AssetType)
                .Include(p => p.AssetLocation)
                .Include(p => p.Department)
                .OrderBy(p => p.PrinterId);

            // Get total count of printers
            var totalPrinters = await printersQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPrinters / (double)pageSize);

            // Get the paginated printers data
            var printers = await printersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Debugging: Print out loaded printers for inspection
            foreach (var printer in printers)
            {
                Console.WriteLine($"Printer: {printer.PrinterId}, AssetType: {printer.AssetType?.Name}, Location: {printer.AssetLocation?.Name}, Department: {printer.Department?.DepartmentName}");
            }

            // Set pagination information in ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            // Return the view with the printers data
            return View(printers);
        }






        // GET: Printers/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Fetch data from the database
            // Fetch data from the database
            var assetTypes = await _context.AssetTypes.ToListAsync() ?? new List<AssetType>();
            var assetLocations = await _context.AssetLocations.ToListAsync() ?? new List<AssetLocation>();
            var departments = await _context.Departments.ToListAsync() ?? new List<Department>();

            // Debug: Check if any of the lists are null or empty
            if (assetTypes == null || !assetTypes.Any())
            {
                // Log or show error message if AssetTypes is null or empty
                Console.WriteLine("No AssetTypes available.");
            }

            if (assetLocations == null || !assetLocations.Any())
            {
                Console.WriteLine("No AssetLocations available.");
            }

            if (departments == null || !departments.Any())
            {
                Console.WriteLine("No Departments available.");
            }

            // Populate ViewData with the fetched data
            ViewData["AssetTypes"] = assetTypes;
            ViewData["AssetLocations"] = assetLocations;
            ViewData["Departments"] = departments;


            return View();
        }





        // POST: Printers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Printer printer)
        {
            // Check if the ModelState is valid
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine($"Error: {error.ErrorMessage}");  // Log errors to Debug Output
                }
                return View(printer);  // Return the form with validation errors
            }

            try
            {
                // Check for existing printer with the same Serial Number (Optional)
                var existingPrinter = await _context.Printers
                                                   .FirstOrDefaultAsync(p => p.SerialNumber == printer.SerialNumber);

                if (existingPrinter != null)
                {
                    Debug.WriteLine("Printer with this serial number already exists.");
                    ModelState.AddModelError("SerialNumber", "This printer already exists.");
                    return View(printer);  // Return to form with error message
                }

                // Add the printer to the database
                _context.Add(printer);
                await _context.SaveChangesAsync();  // Save the new printer record

                TempData["SuccessMessage"] = "Printer added successfully!";  // Set the success message
                Debug.WriteLine("Printer added successfully.");  // Log successful addition
                return RedirectToAction(nameof(Index));  // Redirect to Index page after successful creation
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during the process
                Debug.WriteLine($"Error while saving printer: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the printer.");
                return View(printer);  // Return to the form with the error message
            }
        }


        // If the form has validation errors, re-populate the dropdowns using ViewBag
        //ViewBag.AssetTypes = await _context.AssetTypes.ToListAsync();
        //    ViewBag.AssetLocations = await _context.AssetLocations.ToListAsync();
        //    ViewBag.Departments = await _context.Departments.ToListAsync(); // Add this for Department dropdown

        //    // Return the form with the validation errors
        //    return View(printer);
        //}







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
                .Include(p => p.AssetType)
                .Include(p => p.AssetLocation)
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.PrinterId == id);

            if (printer == null)
            {
                return NotFound();
            }

            // Pass data to the view
            ViewData["AssetTypes"] = await _context.AssetTypes.ToListAsync();
            ViewData["Locations"] = await _context.AssetLocations.ToListAsync();
            ViewData["Departments"] = await _context.Departments.ToListAsync();

            return View(printer);
        }




        // POST: Printers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrinterId,ITAssetTag,Division,AssetTypeId,AssetLocationId,DepartmentId,PrinterMake,PrinterModel,SerialNumber,CartridgeType,Status,GRNNumber,GRNDate,InvoiceDate,Warranty,EndDate")] Printer printer)
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

        private bool PrinterExists(string serialNumber) // Duplicate method
        {
            return _context.Printers.Any(e => e.SerialNumber == serialNumber);
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

            // Declare lists to hold master data that will be added to the database
            List<AssetType> assetTypesToAdd = new List<AssetType>();
            List<AssetLocation> assetLocationsToAdd = new List<AssetLocation>();
            List<Department> departmentsToAdd = new List<Department>();

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

                            // Read columns from the Excel file and map them to the Printer model properties
                            string itAssetTag = worksheet.Cells[row, 1]?.Text?.Trim();
                            string printerMake = worksheet.Cells[row, 2]?.Text?.Trim();
                            string printerModel = worksheet.Cells[row, 3]?.Text?.Trim();
                            string serialNumber = worksheet.Cells[row, 4]?.Text?.Trim();
                            string cartridgeType = worksheet.Cells[row, 5]?.Text?.Trim();
                            string assetTypeText = worksheet.Cells[row, 6]?.Text?.Trim();
                            string assetLocationText = worksheet.Cells[row, 7]?.Text?.Trim();
                            string departmentText = worksheet.Cells[row, 8]?.Text?.Trim();
                            string division = worksheet.Cells[row, 9]?.Text?.Trim();
                            string status = worksheet.Cells[row, 10]?.Text?.Trim();
                            string warranty = worksheet.Cells[row, 11]?.Text?.Trim();
                            string grnDateText = worksheet.Cells[row, 12]?.Text?.Trim();
                            string grnNumber = worksheet.Cells[row, 13]?.Text?.Trim();
                            string invoiceDateText = worksheet.Cells[row, 14]?.Text?.Trim();
                            string endDateText = worksheet.Cells[row, 15]?.Text?.Trim();

                            // Ensure master data exists for AssetType, AssetLocation, and Department

                            // AssetType
                            var assetType = await _context.AssetTypes.FirstOrDefaultAsync(a => a.Name == assetTypeText);
                            if (assetType == null && !string.IsNullOrEmpty(assetTypeText))
                            {
                                assetType = new AssetType { Name = assetTypeText };
                                _context.AssetTypes.Add(assetType);
                                await _context.SaveChangesAsync();
                            }

                            // AssetLocation
                            var location = await _context.AssetLocations.FirstOrDefaultAsync(l => l.Name == assetLocationText);
                            if (location == null && !string.IsNullOrEmpty(assetLocationText))
                            {
                                location = new AssetLocation { Name = assetLocationText };
                                _context.AssetLocations.Add(location);
                                await _context.SaveChangesAsync();
                            }

                            // Department
                            var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == departmentText);
                            if (department == null && !string.IsNullOrEmpty(departmentText))
                            {
                                department = new Department { DepartmentName = departmentText };
                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }

                            // ===== Convert dates =====
                            DateTime? grnDate = null;
                            DateTime? invoiceDate = null;
                            DateTime? endDate = null;

                            if (DateTime.TryParseExact(grnDateText, "dd/MMM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedGrnDate))
                                grnDate = parsedGrnDate;

                            if (DateTime.TryParseExact(invoiceDateText, "dd/MMM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedInvoiceDate))
                                invoiceDate = parsedInvoiceDate;

                            if (DateTime.TryParseExact(endDateText, "dd/MMM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedEndDate))
                                endDate = parsedEndDate;

                            // ===== Create Printer instance =====
                            var printer = new Printer
                            {
                                ITAssetTag = itAssetTag,
                                PrinterMake = printerMake,
                                PrinterModel = printerModel,
                                SerialNumber = serialNumber,
                                CartridgeType = cartridgeType,
                                AssetTypeId = assetType?.AssetTypeId ?? 1,
                                AssetLocationId = location?.AssetLocationId ?? 0, // Ensure AssetLocationId is assigned properly
                                DepartmentId = department?.DepartmentId ?? 0,   // Ensure DepartmentId is assigned properly
                                Division = division,
                                Status = status,
                                Warranty = warranty,
                                GRNDate = grnDate ?? DateTime.MinValue,
                                GRNNumber = grnNumber,
                                InvoiceDate = invoiceDate ?? DateTime.MinValue,
                                EndDate = endDate ?? DateTime.MinValue
                            };

                            printers.Add(printer);  // Add printer to the list for bulk insert
                        }
                    }
                }

                // Save data to the database

                // Remove existing printers if overwriteExisting is true
                if (overwriteExisting)
                    _context.Printers.RemoveRange(_context.Printers);

                // Bulk insert AssetType, AssetLocation, Department data
                if (assetTypesToAdd.Any())
                    _context.AssetTypes.AddRange(assetTypesToAdd);

                if (assetLocationsToAdd.Any())
                    _context.AssetLocations.AddRange(assetLocationsToAdd);

                if (departmentsToAdd.Any())
                    _context.Departments.AddRange(departmentsToAdd);

                // Bulk insert Printer data
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
