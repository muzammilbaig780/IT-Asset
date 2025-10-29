using AssetManagement.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SLRM_IT_Assest_Management.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Account/LoginRegister
        [HttpGet]
        public IActionResult LoginRegister()
        {
            return View(); // unified page for Login + Signup
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users
                    .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Create authentication cookie with claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("Email", user.Email ?? "")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                    );

                    return RedirectToAction("Index", "Home"); // Dashboard after login
                }

                ViewBag.Message = "Invalid username or password.";
            }

            return View("LoginRegister", model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_db.Users.Any(u => u.Username == model.Username))
                {
                    ViewBag.Message = "Username already exists.";
                    return View("LoginRegister", model);
                }

                // Check if email already exists
                if (_db.Users.Any(u => u.Email == model.Email))
                {
                    ViewBag.Message = "Email already registered.";
                    return View("LoginRegister", model);
                }

                // Add new user
                _db.Users.Add(model);
                await _db.SaveChangesAsync();

                // Automatically log in after successful signup
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim("Email", model.Email ?? "")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return RedirectToAction("Index", "Home");
            }

            return View("LoginRegister", model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginRegister", "Account");
        }
    }
}
