using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class PrinterTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrinterTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrinterType
        public async Task<IActionResult> Index()
        {
            var printerTypes = await _context.PrinterTypes.ToListAsync();
            return View(printerTypes);
        }

        // GET: PrinterType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var printerType = await _context.PrinterTypes
                .FirstOrDefaultAsync(m => m.PrinterTypeId == id);
            if (printerType == null)
            {
                return NotFound();
            }

            return View(printerType);
        }

        // GET: PrinterType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrinterType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrinterTypeId,Name")] PrinterType printerType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(printerType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(printerType);
        }

        // GET: PrinterType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var printerType = await _context.PrinterTypes.FindAsync(id);
            if (printerType == null)
            {
                return NotFound();
            }
            return View(printerType);
        }

        // POST: PrinterType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrinterTypeId,Name")] PrinterType printerType)
        {
            if (id != printerType.PrinterTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(printerType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrinterTypeExists(printerType.PrinterTypeId))
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
            return View(printerType);
        }

        // GET: PrinterType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var printerType = await _context.PrinterTypes
                .FirstOrDefaultAsync(m => m.PrinterTypeId == id);
            if (printerType == null)
            {
                return NotFound();
            }

            return View(printerType);
        }

        // POST: PrinterType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var printerType = await _context.PrinterTypes.FindAsync(id);
            _context.PrinterTypes.Remove(printerType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrinterTypeExists(int id)
        {
            return _context.PrinterTypes.Any(e => e.PrinterTypeId == id);
        }
    }
}
