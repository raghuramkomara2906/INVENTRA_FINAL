using System.Linq;
using System.Threading.Tasks;
using ADAProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using Newtonsoft.Json;

namespace ADAProject.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly AppDbContext       _db;
        private readonly IHttpClientFactory _http;

        public SuppliersController(AppDbContext db, IHttpClientFactory http)
        {
            _db   = db;
            _http = http;
        }

        // GET /Suppliers
        public async Task<IActionResult> Index()
        {
            // 1) Fetch all users from JSONPlaceholder
            var clientJson = await _http.CreateClient()
                                        .GetStringAsync("https://jsonplaceholder.typicode.com/users");
            var users      = JsonConvert.DeserializeObject<List<PlaceholderUser>>(clientJson)!;

            // 2) Seed your Suppliers table if empty
            if (!await _db.Suppliers.AnyAsync())
            {
                var toSeed = users.Select(u => new Supplier {
                    Name    = u.Name,
                    Contact = u.Username,
                    Email   = u.Email,
                    Phone   = u.Phone
                });
                _db.Suppliers.AddRange(toSeed);
                await _db.SaveChangesAsync();
            }

            // 3) Read back from Azure and pass to the Razor view
            var allSuppliers = await _db.Suppliers
                                        .OrderBy(s => s.Id)
                                        .ToListAsync();

            return View(allSuppliers);
        }

        // POST /Suppliers/Create  <-- hooked to your modal form asp-action="Create"
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _db.Suppliers.Add(supplier);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Suppliers/Edit   <-- hooked to your modal form asp-action="Edit"
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _db.Suppliers.Update(supplier);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Suppliers/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sup = await _db.Suppliers.FindAsync(id);
            if (sup != null)
            {
                _db.Suppliers.Remove(sup);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Aux class to deserialize JSONPlaceholder users
        private class PlaceholderUser
        {
            public int    Id       { get; set; }
            public string Name     { get; set; } = "";
            public string Username { get; set; } = "";
            public string Email    { get; set; } = "";
            public string Phone    { get; set; } = "";
        }
    }
}