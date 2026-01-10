using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;
using SLRM_IT_Assest_Management.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class ConsumablesController : Controller
    {
        private readonly IConsumableService _consumableService;
        private readonly ApplicationDbContext _context;

        public ConsumablesController(
            IConsumableService consumableService,
            ApplicationDbContext context)
        {
            _consumableService = consumableService;
            _context = context;
        }

        /* ===================== LIST ===================== */
        public async Task<IActionResult> Index()
        {
            var consumables = await _context.Consumables
                .Include(c => c.Stock)
                .AsNoTracking()
                .ToListAsync();

            return View(consumables);
        }

        /* ===================== CREATE ===================== */
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Consumable consumable)
        {
            if (!ModelState.IsValid)
                return View(consumable);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create Consumable
                _context.Consumables.Add(consumable);
                await _context.SaveChangesAsync();

                // Create Stock
                var stock = new ConsumableStock
                {
                    ConsumableId = consumable.ConsumableId,
                    TotalQuantity = consumable.Quantity,
                    AvailableQuantity = consumable.Quantity,
                    LastUpdatedOn = DateTime.Now
                };
                _context.ConsumableStocks.Add(stock);

                // Initial Transaction
                var stockTransaction = new ConsumableTransaction
                {
                    ConsumableId = consumable.ConsumableId,
                    TransactionType = ConsumableTransactionType.StockIn,
                    Quantity = consumable.Quantity,
                    TransactionDate = DateTime.Now,
                    PerformedBy = User.Identity?.Name ?? "System",
                    ReferenceNo = "INITIAL-STOCK",
                    Remarks = "Initial stock entry"
                };
                _context.ConsumableTransactions.Add(stockTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Consumable created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /* ===================== ISSUE ===================== */
        public async Task<IActionResult> Issue()
        {
            ViewBag.Consumables = await _context.Consumables
                .Where(c => c.IsActive)
                .Select(c => new SelectListItem
                {
                    Value = c.ConsumableId.ToString(),
                    Text = c.ConsumableName
                }).ToListAsync();

            ViewBag.Assets = await _context.Assets
                .Select(a => new SelectListItem
                {
                    Value = a.AssetId.ToString(),
                    Text = a.AssetTag
                }).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue(int consumableId, decimal quantity, int assetId, string referenceNo)
        {
            if (consumableId == 0 || quantity <= 0 || assetId == 0 || string.IsNullOrEmpty(referenceNo))
            {
                TempData["ErrorMessage"] = "All fields are required and quantity must be greater than 0.";
                return RedirectToAction(nameof(Issue));
            }

            var stock = await _context.ConsumableStocks.FirstOrDefaultAsync(s => s.ConsumableId == consumableId);
            if (stock == null)
            {
                TempData["ErrorMessage"] = "Consumable stock not found.";
                return RedirectToAction(nameof(Issue));
            }

            if (stock.AvailableQuantity < quantity)
            {
                TempData["ErrorMessage"] = "Insufficient stock available.";
                return RedirectToAction(nameof(Issue));
            }

            stock.AvailableQuantity -= quantity;
            stock.LastUpdatedOn = DateTime.Now;

            _context.ConsumableTransactions.Add(new ConsumableTransaction
            {
                ConsumableId = consumableId,
                AssetId = assetId,
                Quantity = quantity,
                TransactionType = ConsumableTransactionType.Issue,
                TransactionDate = DateTime.Now,
                PerformedBy = User.Identity?.Name ?? "System",
                ReferenceNo = referenceNo,
                Remarks = "Issued to asset"
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Consumable issued successfully.";
            return RedirectToAction(nameof(Index));
        }

        /* ===================== STOCK IN ===================== */
        public async Task<IActionResult> StockIn()
        {
            ViewBag.Consumables = await _context.Consumables
                .Select(c => new SelectListItem
                {
                    Value = c.ConsumableId.ToString(),
                    Text = c.ConsumableName
                }).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockIn(int consumableId, decimal quantity, string referenceNo)
        {
            if (consumableId == 0 || quantity <= 0 || string.IsNullOrEmpty(referenceNo))
            {
                TempData["ErrorMessage"] = "All fields are required and quantity must be greater than 0.";
                return RedirectToAction(nameof(StockIn));
            }

            var stock = await _context.ConsumableStocks.FirstOrDefaultAsync(s => s.ConsumableId == consumableId);
            if (stock == null)
            {
                TempData["ErrorMessage"] = "Consumable stock not found.";
                return RedirectToAction(nameof(StockIn));
            }

            stock.TotalQuantity += quantity;
            stock.AvailableQuantity += quantity;
            stock.LastUpdatedOn = DateTime.Now;

            _context.ConsumableTransactions.Add(new ConsumableTransaction
            {
                ConsumableId = consumableId,
                Quantity = quantity,
                TransactionType = ConsumableTransactionType.StockIn,
                TransactionDate = DateTime.Now,
                ReferenceNo = referenceNo,
                Remarks = "Stock-In entry",
                PerformedBy = User.Identity?.Name ?? "System"
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Stock-In successful";
            return RedirectToAction(nameof(Index));
        }

        /* ===================== RETURN ===================== */
        public async Task<IActionResult> Return(int id)
        {
            var consumable = await _context.Consumables
                .Include(c => c.Stock)
                .FirstOrDefaultAsync(c => c.ConsumableId == id);

            if (consumable == null || consumable.Stock == null)
                return NotFound();

            ViewBag.ConsumableId = consumable.ConsumableId;
            ViewBag.ConsumableName = consumable.ConsumableName;
            ViewBag.MaxQuantity = consumable.Stock.AvailableQuantity;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int consumableId, decimal quantity, string referenceNo, string remarks)
        {
            if (consumableId == 0 || quantity <= 0 || string.IsNullOrEmpty(referenceNo))
            {
                TempData["ErrorMessage"] = "All fields are required and quantity must be greater than 0.";
                return RedirectToAction(nameof(Return), new { id = consumableId });
            }

            var stock = await _context.ConsumableStocks.FirstOrDefaultAsync(s => s.ConsumableId == consumableId);
            if (stock == null)
            {
                TempData["ErrorMessage"] = "Consumable stock not found.";
                return RedirectToAction(nameof(Return), new { id = consumableId });
            }

            stock.AvailableQuantity += quantity;
            stock.LastUpdatedOn = DateTime.Now;

            _context.ConsumableTransactions.Add(new ConsumableTransaction
            {
                ConsumableId = consumableId,
                Quantity = quantity,
                TransactionType = ConsumableTransactionType.Return,
                TransactionDate = DateTime.Now,
                PerformedBy = User.Identity?.Name ?? "System",
                ReferenceNo = referenceNo,
                Remarks = string.IsNullOrEmpty(remarks) ? "Returned from asset" : remarks
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Consumable returned successfully!";

            return RedirectToAction(nameof(Index));
        }


        /* ===================== TRANSACTIONS LEDGER ===================== */
        public async Task<IActionResult> Ledger(int id)
        {
            // Check if the consumable exists
            var consumable = await _context.Consumables
                .FirstOrDefaultAsync(c => c.ConsumableId == id);

            if (consumable == null)
                return NotFound();

            // Get all transactions for this consumable
            var transactions = await _context.ConsumableTransactions
                .Include(t => t.Asset) // optional, only if you have Asset linked
                .Where(t => t.ConsumableId == id)
                .OrderByDescending(t => t.TransactionDate)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.ConsumableName = consumable.ConsumableName;

            return View(transactions);
        }


    }
}
