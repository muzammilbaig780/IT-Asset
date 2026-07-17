
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;
using AssetManagement.Data;

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
        return View(await _context.StockInventories.ToListAsync());
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
        return View();
    }

    // POST: STOCKINVENTORYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StoreInventoryId,ItemName,Category,InvoiceNumber,InvoiceDate,ReceivedQty,AvailableQty,IssuedQty,StoreLocation,ReceivedBy,Remarks,CreatedDate,CreatedBy")] StockInventory stockinventory)
    {
        if (ModelState.IsValid)
        {
            _context.Add(stockinventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
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

    private bool StockInventoryExists(int? storeinventoryid)
    {
        return _context.StockInventories.Any(e => e.StoreInventoryId == storeinventoryid);
    }
}
