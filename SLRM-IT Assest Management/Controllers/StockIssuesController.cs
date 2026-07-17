
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;
using AssetManagement.Data;

public class StockIssuesController : Controller
{
    private readonly ApplicationDbContext _context;

    public StockIssuesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: STOCKISSUES
    public async Task<IActionResult> Index()
    {
        var stockIssues = await _context.StockIssues
            .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemNameMaster)
            .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemCodeMaster)
            .Include(x => x.Department)
            .ToListAsync();

        return View(stockIssues);
    }

    // GET: STOCKISSUES/Details/5
    public async Task<IActionResult> Details(int? issueid)
    {
        if (issueid == null)
        {
            return NotFound();
        }

        var stockissue = await _context.StockIssues
            .FirstOrDefaultAsync(m => m.IssueId == issueid);
        if (stockissue == null)
        {
            return NotFound();
        }

        return View(stockissue);
    }

    // GET: STOCKISSUES/Create
    public IActionResult Create()
    {
        ViewBag.StockItems = _context.StockInventories
            .Include(x => x.ItemNameMaster)
            .Include(x => x.ItemCodeMaster)
            .ToList();

        ViewBag.Departments = _context.Departments
            .OrderBy(x => x.DepartmentName)
            .ToList();

        return View();
    }

    // POST: STOCKISSUES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StockIssue stockissue)
    {
        if (ModelState.IsValid)
        {
            stockissue.RequestDate = DateTime.Now;

            stockissue.Level1Status = "Pending";
            stockissue.Level2Status = "Pending";
            stockissue.Status = "Pending";

            stockissue.Level1ApprovedBy = null;
            stockissue.Level2ApprovedBy = null;
            stockissue.IssuedBy = null;

            _context.StockIssues.Add(stockissue);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Request submitted successfully.";

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Departments = _context.Departments.OrderBy(x => x.DepartmentName).ToList();
        ViewBag.StockItems = _context.StockInventories
       .Include(x => x.ItemNameMaster)
       .Include(x => x.ItemCodeMaster)
       .ToList();

        return View(stockissue);
    }


    public async Task<IActionResult> Level1Approval()
    {
        var requests = await _context.StockIssues
            .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemNameMaster)
            .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemCodeMaster)
            .Include(x => x.Department)
            .Where(x => x.Level1Status == "Pending")
            .ToListAsync();

        return View(requests);
    }

    public async Task<IActionResult> ApproveLevel1(int id)
    {
        var request = await _context.StockIssues.FindAsync(id);

        if (request == null)
            return NotFound();

        request.Level1Status = "Approved";
        request.Level1ApprovedBy = User.Identity?.Name ?? "Admin";
        request.Level1ApprovedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Level 1 Approved.";

        return RedirectToAction(nameof(Level1Approval));
    }

    [HttpPost]
    public async Task<IActionResult> RejectLevel1(int id, string remarks)
    {
        var request = await _context.StockIssues.FindAsync(id);

        if (request == null)
            return NotFound();

        request.Level1Status = "Rejected";
        request.Status = "Rejected";
        request.Level1Remarks = remarks;
        request.Level1ApprovedBy = User.Identity?.Name ?? "Admin";
        request.Level1ApprovedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Request Rejected.";

        return RedirectToAction(nameof(Level1Approval));
    }

    public async Task<IActionResult> Level2Approval()
    {
        var requests = await _context.StockIssues
            .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemNameMaster)
            .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemCodeMaster)
            .Include(x => x.Department)
            .Where(x => x.Level1Status == "Approved"
                     && x.Level2Status == "Pending")
            .ToListAsync();

        return View(requests);
    }

    public async Task<IActionResult> ApproveLevel2(int id)
    {
        var request = await _context.StockIssues.FindAsync(id);

        if (request == null)
            return NotFound();

        request.Level2Status = "Approved";
        request.Level2ApprovedBy = User.Identity?.Name ?? "Store Manager";
        request.Level2ApprovedDate = DateTime.Now;

        request.Status = "Approved";

        await _context.SaveChangesAsync();

        TempData["Success"] = "Level 2 Approved.";

        return RedirectToAction(nameof(Level2Approval));
    }

    [HttpPost]
    public async Task<IActionResult> RejectLevel2(int id, string remarks)
    {
        var request = await _context.StockIssues.FindAsync(id);

        if (request == null)
            return NotFound();

        request.Level2Status = "Rejected";
        request.Level2ApprovedBy = User.Identity?.Name ?? "Store Manager";
        request.Level2ApprovedDate = DateTime.Now;
        request.Level2Remarks = remarks;

        request.Status = "Rejected";

        await _context.SaveChangesAsync();

        TempData["Success"] = "Request Rejected.";

        return RedirectToAction(nameof(Level2Approval));
    }

    public async Task<IActionResult> IssueStock()
    {
        var list = await _context.StockIssues
              .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemNameMaster)
            .Include(x => x.StockInventory)
                .ThenInclude(x => x.ItemCodeMaster)
            .Include(x => x.Department)
            .Where(x =>
                x.Level1Status == "Approved" &&
                x.Level2Status == "Approved" &&
                x.Status == "Approved")
            .ToListAsync();

        return View(list);
    }

    public async Task<IActionResult> Issue(int id)
    {
        var request = await _context.StockIssues
            .Include(x => x.StockInventory)
            .FirstOrDefaultAsync(x => x.IssueId == id);

        if (request == null)
            return NotFound();

        if (request.Level1Status != "Approved" ||
            request.Level2Status != "Approved")
        {
            TempData["Error"] = "Both approvals are required before issuing stock.";
            return RedirectToAction(nameof(IssueStock));
        }

        if (request.StockInventory.AvailableQty < request.IssueQty)
        {
            TempData["Error"] = "Insufficient Stock.";
            return RedirectToAction(nameof(IssueStock));
        }

        request.StockInventory.AvailableQty -= request.IssueQty;

        request.Status = "Issued";
        request.IssuedBy = User.Identity?.Name ?? "Store Keeper";
        request.IssuedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Stock Issued Successfully.";

        return RedirectToAction(nameof(IssueStock));
    }

    // GET: STOCKISSUES/Edit/5
    public async Task<IActionResult> Edit(int? issueid)
    {
        if (issueid == null)
        {
            return NotFound();
        }

        var stockissue = await _context.StockIssues.FindAsync(issueid);
        if (stockissue == null)
        {
            return NotFound();
        }
        return View(stockissue);
    }

    // POST: STOCKISSUES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? issueid, [Bind("IssueId,StoreInventoryId,StockInventory,DepartmentId,Department,EmployeeName,EmployeeCode,IssueQty,RequestDate,RequestedBy,Level1Status,Level1ApprovedBy,Level1ApprovedDate,Level1Remarks,Level2Status,Level2ApprovedBy,Level2ApprovedDate,Level2Remarks,Status,IssuedDate,IssuedBy,Remarks")] StockIssue stockissue)
    {
        if (issueid != stockissue.IssueId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(stockissue);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockIssueExists(stockissue.IssueId))
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
        return View(stockissue);
    }

    // GET: STOCKISSUES/Delete/5
    public async Task<IActionResult> Delete(int? issueid)
    {
        if (issueid == null)
        {
            return NotFound();
        }

        var stockissue = await _context.StockIssues
            .FirstOrDefaultAsync(m => m.IssueId == issueid);
        if (stockissue == null)
        {
            return NotFound();
        }

        return View(stockissue);
    }

    // POST: STOCKISSUES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? issueid)
    {
        var stockissue = await _context.StockIssues.FindAsync(issueid);
        if (stockissue != null)
        {
            _context.StockIssues.Remove(stockissue);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StockIssueExists(int? issueid)
    {
        return _context.StockIssues.Any(e => e.IssueId == issueid);
    }
}
