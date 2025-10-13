using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using SLRM_IT_Assest_Management.Models;

public class MasterDataController : Controller
{
    private readonly ApplicationDbContext _context;

    public MasterDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ========== COMPANY ==========
    [HttpGet]
    public IActionResult CreateCompany()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany(Company company)
    {
        if (ModelState.IsValid)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Company added!";
            return RedirectToAction("Create", "Assets");
        }
        return View(company);
    }

    // ========== ASSET TYPE ==========
    [HttpGet]
    public IActionResult CreateAssetType()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetType(AssetType assetType)
    {
        if (ModelState.IsValid)
        {
            _context.AssetTypes.Add(assetType);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Asset type added!";
            return RedirectToAction("Create", "Assets");
        }
        return View(assetType);
    }

    // ========== ASSET LOCATION ==========
    [HttpGet]
    public IActionResult CreateAssetLocation()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetLocation(AssetLocation location)
    {
        if (ModelState.IsValid)
        {
            _context.AssetLocations.Add(location);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Location added!";
            return RedirectToAction("Create", "Assets");
        }
        return View(location);
    }

    // ========== STATUS ==========
    [HttpGet]
    public IActionResult CreateStatus()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatus(Status status)
    {
        if (ModelState.IsValid)
        {
            _context.AssetStatuses.Add(status);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Status added!";
            return RedirectToAction("Create", "Assets");
        }
        return View(status);
    }
}
