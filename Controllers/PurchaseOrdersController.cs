// Controllers/PurchaseOrdersController.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using ADAProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ADAProject.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private readonly AppDbContext      _db;
        private readonly IHttpClientFactory _http;

        public PurchaseOrdersController(AppDbContext db, IHttpClientFactory http)
        {
            _db   = db;
            _http = http;
        }

        // GET /PurchaseOrders?status=All
        public async Task<IActionResult> Index(string status = "All")
        {
            // 1) fetch carts
            var client   = _http.CreateClient();
            var json     = await client.GetStringAsync("https://dummyjson.com/carts?limit=100");
            var response = JsonConvert.DeserializeObject<DummyJsonCartResponse>(json)!;

            // 2) seed first 20 if empty
            if (!await _db.PurchaseOrders.AnyAsync())
            {
                // load products for thumbnails
                var products = await _db.Products.ToListAsync();

                var toSeed = response.Carts
                                     .Take(20)
                                     .Select(c =>
                                     {
                                         var firstProdId = c.Products.First().Id;
                                         var prod        = products.FirstOrDefault(p => p.Id == firstProdId);

                                         return new PurchaseOrder
                                         {
                                             Id      = c.Id.ToString(),
                                             Details = string.Join("||",         // we'll split in the view
                                                   c.Products.Select(p => $"{p.Quantity}× {p.Title}")),
                                             Status  = "Draft",
                                         };
                                     });

                _db.PurchaseOrders.AddRange(toSeed);
                await _db.SaveChangesAsync();
            }

            // 3) filter
            var q = _db.PurchaseOrders.AsQueryable();
            if (status != "All")
                q = q.Where(po => po.Status == status);

            var list = await q.OrderBy(po => Convert.ToInt32(po.Id)).ToListAsync();
            ViewBag.StatusFilter = status;
            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrder po)
        {
            if (ModelState.IsValid)
            {
                _db.PurchaseOrders.Add(po);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PurchaseOrder po)
        {
            if (ModelState.IsValid)
            {
                _db.PurchaseOrders.Update(po);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var po = await _db.PurchaseOrders.FindAsync(id);
            if (po != null)
            {
                _db.PurchaseOrders.Remove(po);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
