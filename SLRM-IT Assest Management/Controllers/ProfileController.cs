using AssetManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SLRM_IT_Assest_Management.Models;
using System;
using System.Linq;

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
        public IActionResult Index()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = _db.UserProfile
                          .Include(u => u.Department)
                          .FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                // Ensure there's at least one department
                var defaultDepartment = _db.Departments.FirstOrDefault();
                if (defaultDepartment == null)
                {
                    // Create one if none exist
                    defaultDepartment = new Department { DepartmentName = "General" };
                    _db.Departments.Add(defaultDepartment);
                    _db.SaveChanges();
                }

                user = new UserProfile
                {
                    FullName = "New User",
                    Email = userEmail,
                    DepartmentId = defaultDepartment.DepartmentId, // <-- Use DepartmentId, not Id
                    Role = "User",
                    ProfilePicturePath = ""
                };

                _db.UserProfile.Add(user);
                _db.SaveChanges();
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
        public IActionResult Edit(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                // Check if the DepartmentId exists in the Departments table
                var department = _db.Departments.FirstOrDefault(d => d.Id == model.DepartmentId);
                if (department == null)
                {
                    ModelState.AddModelError("DepartmentId", "The selected department does not exist.");
                    return View(model);
                }

                var user = _db.UserProfile.Find(model.Id);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.DepartmentId = model.DepartmentId; // Make sure it's a valid DepartmentId
                    user.Role = model.Role;
                    _db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }


    }
}
