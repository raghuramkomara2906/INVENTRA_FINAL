// Controllers/AuthController.cs
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ADAProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ADAProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        [HttpPost]
        [Route("api/signup")]
        public async Task<IActionResult> SignUp([FromForm] string Username, [FromForm] string Password)
        {
            if (await _db.Users.AnyAsync(u => u.UserName == Username))
                return BadRequest("Username exists.");

            // simple PBKDF2 hash
            byte[] salt = new byte[128/8];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt:     salt,
                prf:      KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256/8));

            var user = new User {
              UserName     = Username,
              PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}"
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok("Account created");
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromForm] string Login_UserName, [FromForm] string Login_Password)
        {
            var user = await _db.Users
                        .FirstOrDefaultAsync(u => u.UserName == Login_UserName);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var parts = user.PasswordHash.Split(':');
            var salt  = Convert.FromBase64String(parts[0]);
            var hash  = parts[1];

            var attempted = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Login_Password,
                salt:     salt,
                prf:      KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256/8));

            if (attempted != hash)
                return Unauthorized("Invalid credentials");

            // create cookie
            var claims = new[] {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                  new ClaimsPrincipal(id));

            return Ok();
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
