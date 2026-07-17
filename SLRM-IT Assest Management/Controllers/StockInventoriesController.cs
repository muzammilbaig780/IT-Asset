using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class StockInventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockInventoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: STOCKINVENTORYS
        public async Task<IActionResult> Index()
        {
            var stocks = await _context.StockInventories
                .Include(x => x.ItemNameMaster)
                .Include(x => x.ItemCodeMaster)
                .ToListAsync();

            ViewBag.TotalItems = stocks.Count;

            ViewBag.TotalReceived = stocks.Sum(x => x.ReceivedQty);

            ViewBag.TotalAvailable = stocks.Sum(x => x.AvailableQty);

            ViewBag.TotalIssued = stocks.Sum(x => x.ReceivedQty - x.AvailableQty);

            DateTime alertDate = DateTime.Now.AddDays(-15);

            ViewBag.AlertCount = stocks.Count(x =>
                x.AvailableQty > 0 &&
                x.CreatedDate <= alertDate);

            return View(stocks);
        }


        public IActionResult GetAlertItems()
        {
            var items = _context.StockInventories
                .Include(x => x.ItemNameMaster)
                .Include(x => x.ItemCodeMaster)
                .ToList();

            return PartialView("AlertItems", items);
        }
        // GET: STOCKINVENTORYS/Details/5
        public async Task<IActionResult> Details(int? storeinventoryid)
        {
            if (storeinventoryid == null)
            {
                return NotFound();
            }

            var stockinventory = await _context.StockInventories
                .FirstOrDefaultAsync(m => m.StoreInventoryId == storeinventoryid);
            if (stockinventory == null)
            {
                return NotFound();
            }

            return View(stockinventory);
        }

        // GET: STOCKINVENTORYS/Create
        public IActionResult Create()
        {

            ViewBag.ItemNameMasters = _context.ItemNameMasters
                .OrderBy(x => x.ItemName)
                .ToList();

            ViewBag.ItemCodeMasters = _context.ItemCodeMasters
               .OrderBy(x => x.ItemCode)
               .ToList();


            return View();
        }

        // POST: STOCKINVENTORYS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockInventory stockinventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockinventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            ViewBag.ItemNameMasters = _context.ItemNameMasters
                .OrderBy(x => x.ItemName)
                .ToList();
            ViewBag.ItemCodeMasters = new List<ItemCodeMaster>();

            return View(stockinventory);
        }

        // GET: STOCKINVENTORYS/Edit/5
        public async Task<IActionResult> Edit(int? storeinventoryid)
        {
            if (storeinventoryid == null)
            {
                return NotFound();
            }

            var stockinventory = await _context.StockInventories.FindAsync(storeinventoryid);
            if (stockinventory == null)
            {
                return NotFound();
            }
            return View(stockinventory);
        }

        // POST: STOCKINVENTORYS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? storeinventoryid, [Bind("StoreInventoryId,ItemName,Category,InvoiceNumber,InvoiceDate,ReceivedQty,AvailableQty,IssuedQty,StoreLocation,ReceivedBy,Remarks,CreatedDate,CreatedBy")] StockInventory stockinventory)
        {
            if (storeinventoryid != stockinventory.StoreInventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockinventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockInventoryExists(stockinventory.StoreInventoryId))
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
            return View(stockinventory);
        }

        // GET: STOCKINVENTORYS/Delete/5
        public async Task<IActionResult> Delete(int? storeinventoryid)
        {
            if (storeinventoryid == null)
            {
                return NotFound();
            }

            var stockinventory = await _context.StockInventories
                .FirstOrDefaultAsync(m => m.StoreInventoryId == storeinventoryid);
            if (stockinventory == null)
            {
                return NotFound();
            }

            return View(stockinventory);
        }

        // POST: STOCKINVENTORYS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? storeinventoryid)
        {
            var stockinventory = await _context.StockInventories.FindAsync(storeinventoryid);
            if (stockinventory != null)
            {
                _context.StockInventories.Remove(stockinventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult GetItemCodes(int itemNameMasterId)
        {
            var itemCodes = _context.ItemCodeMasters
                .Where(x => x.ItemNameMasterId == itemNameMasterId)
                .OrderBy(x => x.ItemCode)
                .Select(x => new
                {
                    itemCodeMasterId = x.ItemCodeMasterId,
                    itemCode = x.ItemCode
                })
                .ToList();

            return Json(itemCodes);
        }

        private bool StockInventoryExists(int? storeinventoryid)
        {
            return _context.StockInventories.Any(e => e.StoreInventoryId == storeinventoryid);
        }
    }
}
