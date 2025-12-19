using AssetManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SLRM_IT_Assest_Management.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Profile/
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _db.UserProfile
                .Include(u => u.Department)  // Include related Department data
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                // Ensure there's at least one department
                var defaultDepartment = await _db.Departments.FirstOrDefaultAsync();
                if (defaultDepartment == null)
                {
                    // Create one if none exist
                    defaultDepartment = new Department { DepartmentName = "General" };
                    _db.Departments.Add(defaultDepartment);
                    await _db.SaveChangesAsync();
                }

                user = new UserProfile
                {
                    FullName = "New User",
                    Email = userEmail,
                    DepartmentId = defaultDepartment.DepartmentId, // <-- Use DepartmentId
                    Role = "User",
                    ProfilePicturePath = ""
                };

                _db.UserProfile.Add(user);
                await _db.SaveChangesAsync();
            }

            return View(user);
        }

        // GET: /Profile/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _db.UserProfile.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /Profile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                // Check if the DepartmentId exists in the Departments table
                var department = await _db.Departments.FirstOrDefaultAsync(d => d.Id == model.DepartmentId);
                if (department == null)
                {
                    ModelState.AddModelError("DepartmentId", "The selected department does not exist.");
                    return View(model);
                }

                var user = await _db.UserProfile.FindAsync(model.Id);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.DepartmentId = model.DepartmentId; // Make sure it's a valid DepartmentId
                    user.Role = model.Role;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
