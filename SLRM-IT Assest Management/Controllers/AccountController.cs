using AssetManagement.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SLRM_IT_Assest_Management.Models;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SLRM_IT_Assest_Management.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext db, IConfiguration configuration) // <- Inject here
        {
            _db = db;
            _configuration = configuration; // <- Assign it here
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
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Username and Password are required.";
                return View();
            }

            var user = _db.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

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

                return RedirectToAction("Index", "Home"); // Redirect to the dashboard after login
            }

            // Invalid login
            ViewBag.Message = "Invalid username or password.";
            return View("LoginRegister");// Return to login page with error message
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

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(); // Show Forgot Password page with email input
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Email is required.";
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                // Generate a password reset token (for demo, we'll use a simple GUID)
                var resetToken = Guid.NewGuid().ToString();
                // You would ideally store the reset token in your DB with an expiry time

                // Send email with reset token
                var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Scheme);

                await SendPasswordResetEmail(user.Email, resetLink);

                // Inform the user that an email has been sent
                ViewBag.Message = "Password reset link has been sent to your email.";
            }
            else
            {
                ViewBag.Message = "No account found with this email address.";
            }

            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ForgotPassword");
            }

            // Here you would validate the token from the database before showing the reset password form
            return View(); // Show reset password form
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                ViewBag.Message = "Invalid token or password.";
                return View();
            }

            // Here you would validate the token (e.g., check its expiry, match it in the DB, etc.)

            // Find user by token (simplified example)
            var user = _db.Users.FirstOrDefault(u => u.Email == "user@example.com"); // Replace with actual logic

            if (user != null)
            {
                user.Password = newPassword; // Save the new password (ensure it's hashed)
                await _db.SaveChangesAsync();
                ViewBag.Message = "Your password has been reset successfully.";
                return RedirectToAction("LoginRegister");
            }

            ViewBag.Message = "Invalid token.";
            return View();
        }

        // Send email for password reset
      
        


        // Send email for password reset using SendGrid
        private async Task SendPasswordResetEmail(string email, string resetLink)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
            var subject = "Password Reset Request";
            var to = new EmailAddress(email);
            var plainTextContent = $"To reset your password, click the following link: {resetLink}";
            var htmlContent = $"<p>To reset your password, click the following link: <a href='{resetLink}'>Reset Password</a></p>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
}
