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
    public class ITAssetDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ITAssetDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ITAssetDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.ITAssetDetails.ToListAsync());
        }

        // GET: ITAssetDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iTAssetDetail = await _context.ITAssetDetails
                .FirstOrDefaultAsync(m => m.ITAssetDetailId == id);
            if (iTAssetDetail == null)
            {
                return NotFound();
            }

            return View(iTAssetDetail);
        }

        // GET: ITAssetDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ITAssetDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ITAssetDetailId,SlNo,UserName,Department,Division,AssetLocation,AssetType,Status,Make,Model,SerialNo,TelephoneNo,ParallelConnection,ScreenSize,FrequencyNo,LicenseNo,Ports")] ITAssetDetail iTAssetDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(iTAssetDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(iTAssetDetail);
        }

        // GET: ITAssetDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iTAssetDetail = await _context.ITAssetDetails.FindAsync(id);
            if (iTAssetDetail == null)
            {
                return NotFound();
            }
            return View(iTAssetDetail);
        }

        // POST: ITAssetDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ITAssetDetailId,SlNo,UserName,Department,Division,AssetLocation,AssetType,Status,Make,Model,SerialNo,TelephoneNo,ParallelConnection,ScreenSize,FrequencyNo,LicenseNo,Ports")] ITAssetDetail iTAssetDetail)
        {
            if (id != iTAssetDetail.ITAssetDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(iTAssetDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ITAssetDetailExists(iTAssetDetail.ITAssetDetailId))
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
            return View(iTAssetDetail);
        }

        // GET: ITAssetDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iTAssetDetail = await _context.ITAssetDetails
                .FirstOrDefaultAsync(m => m.ITAssetDetailId == id);
            if (iTAssetDetail == null)
            {
                return NotFound();
            }

            return View(iTAssetDetail);
        }

        // POST: ITAssetDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var iTAssetDetail = await _context.ITAssetDetails.FindAsync(id);
            if (iTAssetDetail != null)
            {
                _context.ITAssetDetails.Remove(iTAssetDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ITAssetDetailExists(int id)
        {
            return _context.ITAssetDetails.Any(e => e.ITAssetDetailId == id);
        }


        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile excelFile, string assetType)
        {
            if (excelFile == null || excelFile.Length == 0 || string.IsNullOrEmpty(assetType))
            {
                TempData["Error"] = "Please select Asset Type and Excel file.";
                return RedirectToAction("Index");
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Skip header
                    {
                        string excelAssetType = worksheet.Cells[row, 6].Text.Trim(); // AssetType Column

                        // ✅ IMPORT ONLY SELECTED ASSET TYPE
                        if (!excelAssetType.Equals(assetType, StringComparison.OrdinalIgnoreCase))
                            continue;

                        var asset = new ITAssetDetail
                        {
                            SlNo = int.TryParse(worksheet.Cells[row, 1].Text, out var sl) ? sl : 0,
                            UserName = worksheet.Cells[row, 2].Text,
                            Department = worksheet.Cells[row, 3].Text,
                            Division = worksheet.Cells[row, 4].Text,
                            AssetLocation = worksheet.Cells[row, 5].Text,
                            AssetType = excelAssetType,
                            Status = worksheet.Cells[row, 7].Text,
                            Make = worksheet.Cells[row, 8].Text,
                            Model = worksheet.Cells[row, 9].Text,
                            SerialNo = worksheet.Cells[row, 10].Text,
                            TelephoneNo = worksheet.Cells[row, 11].Text,
                            ParallelConnection = worksheet.Cells[row, 12].Text,
                            ScreenSize = worksheet.Cells[row, 13].Text,
                            FrequencyNo = worksheet.Cells[row, 14].Text,
                            LicenseNo = worksheet.Cells[row, 15].Text,
                            Ports = worksheet.Cells[row, 16].Text
                        };

                        _context.ITAssetDetails.Add(asset);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = $"{assetType} Excel imported successfully!";
            return RedirectToAction("Index");
        }


    }
}
