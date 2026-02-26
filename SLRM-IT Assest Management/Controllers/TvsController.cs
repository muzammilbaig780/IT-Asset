using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using SLRM_IT_Assest_Management.Models;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class TvsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TvsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tvs
        public async Task<IActionResult> Index()
        {
            var tvs = await _context.Tv
                .Include(t => t.AssetLocation)  // Eager load AssetLocation
                .Include(t => t.Department)      // Eager load Department
                .ToListAsync();

            return View(tvs);
        }
        // GET: Tvs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tv = await _context.Tv
                .Include(t => t.AssetLocation)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(m => m.TvId == id);
            if (tv == null)
            {
                return NotFound();
            }

            return View(tv);
        }

        public async Task<IActionResult> Create()
        {
            // Load Asset Locations
            var assetLocations = await _context.AssetLocations.ToListAsync(); // or your data access method
            ViewBag.AssetLocations = assetLocations;

            // Load Departments
            var departments = await _context.Departments.ToListAsync(); // or your data access method
            ViewBag.Departments = departments;

            return View();
        }

        // POST: Tvs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TvId,AssetLocationId,TvSerialNo,Model,ScrrenSize,DepartmentId,UserName,Qty,Status")] Tv tv)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetLocationId"] = new SelectList(_context.AssetLocations, "AssetLocationId", "Name", tv.AssetLocationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tv.DepartmentId);
            return View(tv);
        }

        // GET: Tvs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tv = await _context.Tv.FindAsync(id);
            if (tv == null)
            {
                return NotFound();
            }
            ViewData["AssetLocationId"] = new SelectList(_context.AssetLocations, "AssetLocationId", "Name", tv.AssetLocationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tv.DepartmentId);
            return View(tv);
        }

        // POST: Tvs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TvId,AssetLocationId,TvSerialNo,Model,ScrrenSize,DepartmentId,UserName,Qty,Status")] Tv tv)
        {
            if (id != tv.TvId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TvExists(tv.TvId))
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
            ViewData["AssetLocationId"] = new SelectList(_context.AssetLocations, "AssetLocationId", "Name", tv.AssetLocationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tv.DepartmentId);
            return View(tv);
        }

        // GET: Tvs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tv = await _context.Tv
                .Include(t => t.AssetLocation)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(m => m.TvId == id);
            if (tv == null)
            {
                return NotFound();
            }

            return View(tv);
        }

        // POST: Tvs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tv = await _context.Tv.FindAsync(id);
            if (tv != null)
            {
                _context.Tv.Remove(tv);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TvExists(int id)
        {
            return _context.Tv.Any(e => e.TvId == id);
        }
    }
}
