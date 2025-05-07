using ADAProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ADAProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword)
        {
            const string pwPolicy = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$";
            if (!Regex.IsMatch(password, pwPolicy))
            {
                ViewBag.Error = 
                "Password must be 8+ chars and include upper + lower + digit + symbol.";
                return View("Index");
            }
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View("Index");
            }

            if (await _db.Users.AnyAsync(u => u.UserName == username))
            {
                ViewBag.Error = "Username already exists.";
                return View("Index");
            }

            var user = new User {
                UserName     = username,
                PasswordHash = ComputeHash(password),
                Email        = ""
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["SignupSuccess"] = "Account created! Please log in.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string loginUserName, string loginPassword)
        {
            var hash = ComputeHash(loginPassword);
            var user = await _db.Users
                                .FirstOrDefaultAsync(u => u.UserName == loginUserName
                                                       && u.PasswordHash == hash);
            if (user == null)
            {
                ViewBag.LoginError = "Invalid credentials";
                return View("Index");
            }

            HttpContext.Session.SetString("CurrentUser", user.UserName);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int id, string fullName, string email)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.UserName = fullName;
            user.Email    = email;
            await _db.SaveChangesAsync();

            TempData["ProfileUpdateSuccess"] = "Profile updated!";
            return RedirectToAction("Index", "Home");
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Account");
        }
    }
}
