using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;

public class ITAssetDetailsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ITAssetDetailsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.ITAssetDetails
            .Include(x => x.Asset)
                .ThenInclude(a => a.Department)
            .Include(x => x.Asset)
                .ThenInclude(a => a.Division)
            .Include(x => x.Asset)
                .ThenInclude(a => a.AssetLocation)
            .Include(x => x.Asset)
                .ThenInclude(a => a.AssetType)
            .Include(x => x.Asset)
                .ThenInclude(a => a.AssetStatus)
            .ToListAsync();

        return View(data);
    }



    // ✅ CREATE (GET)
    public IActionResult Create()
    {
        ViewBag.AssetId = new SelectList(
            _context.Assets.Select(a => new
            {
                a.AssetId,
                Display = a.AssetTag + " - " + a.UserName
            }),
            "AssetId",
            "Display"
        );
        return View();
    }

    // ✅ CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ITAssetDetail model)
    {
        if (ModelState.IsValid)
        {
            _context.ITAssetDetails.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Assets");
        }

        ViewBag.AssetId = new SelectList(_context.Assets, "AssetId", "AssetTag", model.AssetId);
        return View(model);
    }

    // ✅ EDIT (GET)
    public async Task<IActionResult> Edit(int id)
    {
        var data = await _context.ITAssetDetails.FindAsync(id);
        if (data == null)
            return NotFound();

        ViewBag.AssetId = new SelectList(_context.Assets, "AssetId", "AssetTag", data.AssetId);
        return View(data);
    }

    // ✅ EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ITAssetDetail model)
    {
        if (id != model.ITAssetDetailId)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Assets");
        }

        ViewBag.AssetId = new SelectList(_context.Assets, "AssetId", "AssetTag", model.AssetId);
        return View(model);
    }

    // ✅ DELETE (GET)
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.ITAssetDetails
            .Include(x => x.Asset)
            .FirstOrDefaultAsync(x => x.ITAssetDetailId == id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    // ✅ DELETE (POST)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.ITAssetDetails.FindAsync(id);
        _context.ITAssetDetails.Remove(data);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Assets");
    }
}
