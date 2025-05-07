using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADAProject.Models;

namespace ADAProject.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) => _db = db;

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                Products = await _db.Products.ToListAsync(),
                Orders   = await _db.PurchaseOrders.ToListAsync()
            };
            return View(vm);
        }
    }
}
