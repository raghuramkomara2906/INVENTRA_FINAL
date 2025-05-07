using System;
using System.Linq;
using System.Threading.Tasks;
using ADAProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ADAProject.Controllers
{
    public class SalesController : Controller
    {
        private readonly AppDbContext       _db;
        private readonly IHttpClientFactory _http;

        public SalesController(AppDbContext db, IHttpClientFactory http)
        {
            _db   = db;
            _http = http;
        }

        // GET /Sales?status=All
        public async Task<IActionResult> Index(string status = "All")
        {
            // 1) Fetch carts from DummyJSON
            var client = _http.CreateClient();
            var json   = await client.GetStringAsync("https://dummyjson.com/carts?limit=100");
            var resp   = JsonConvert.DeserializeObject<DummyJsonCartResponse>(json)!;
            var rand = new Random();

            // 2) Seed first 20 orders if DB empty
            if (!await _db.Orders.AnyAsync())
            {
                var products = await _db.Products.ToListAsync();

                var toSeed = resp.Carts
                                 .Take(20)
                                 .Select(c =>
                                 {
                                     var firstProdId = c.Products.First().Id;
                                     var prod       = products.FirstOrDefault(p => p.Id == firstProdId);

                                     var monthsAgo = rand.Next(0, 12);
                                     var daysAgo   = rand.Next(0, 30);
                                     return new Order
                                     {
                                         Id     = c.Id.ToString(),
                                         Text   = string.Join(", ",
                                                      c.Products.Select(p => $"{p.Quantity}× {p.Title}")
                                                  ),
                                         Items  = c.TotalQuantity,
                                         Cost   = (decimal)c.Total,
                                         Status = "New",
                                         CreatedAt = DateTime.UtcNow
                                                    .AddMonths(-monthsAgo)
                                                    .AddDays(-daysAgo)
                                     };
                                 });

                _db.Orders.AddRange(toSeed);
                await _db.SaveChangesAsync();
            }

            // 3) Load & filter
            var q = _db.Orders.AsQueryable();
            if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
                q = q.Where(o => o.Status == status);

            var orders = await q
                      .OrderBy(o => o.Id)    // no Convert.ToInt32
                      .ToListAsync();

            ViewBag.StatusFilter = status;
            return View(orders);
        }

        // POST /Sales/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            // 1) If no Id supplied, generate one *before* we validate
            if (string.IsNullOrWhiteSpace(order.Id))
            {
                order.Id = Guid.NewGuid().ToString();
                // drop the missing‐Id error from ModelState
                ModelState.Remove(nameof(order.Id));
            }

            // 2) Now validate
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(kvp => kvp.Value.Errors
                                      .Select(err => $"{kvp.Key}: {err.ErrorMessage}"))
                    .ToArray();
                throw new Exception("Model binding failed:\n" + string.Join("\n", errors));
            }

            // 3) Set default status & save
            order.Status ??= "New";
            order.CreatedAt  = DateTime.UtcNow;
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST /Sales/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Sales/Delete/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var o = await _db.Orders.FindAsync(id);
            if (o != null)
            {
                _db.Orders.Remove(o);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
