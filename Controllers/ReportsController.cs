using System;
using System.Linq;
using System.Threading.Tasks;
using ADAProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADAProject.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _db;
        public ReportsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var vm = new ReportsViewModel();

            // 1) Monthly turnover
            var lastYear = DateTime.Today.AddYears(-1);
            var salesByMonth = await _db.Orders
              .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
              .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
              .Select(g => new {
                Label = $"{g.Key.Month}/{g.Key.Year % 100:00}",
                Total = g.Sum(o => o.Cost)
              })
              .ToListAsync();
            vm.MonthlyLabels   = salesByMonth.Select(x => x.Label).ToList();
            vm.MonthlyTurnover = salesByMonth.Select(x => x.Total).ToList();

            // 2) Category counts
            vm.CategoryCounts = await _db.Products
              .GroupBy(p => p.Genre)
              .Select(g => new {
                Category = g.Key ?? "<Uncategorized>",
                Count    = g.Sum(p => p.Count)
              })
              .ToDictionaryAsync(x => x.Category, x => x.Count);

            // 3) Weekly actual vs forecast (dummy)
            vm.WeekLabels    = Enumerable.Range(0, 7)
              .Select(i => DateTime.Today.AddDays(-6 + i).ToString("MM/dd"))
              .ToList();
            // for demo, random data
            var rnd = new Random();
            vm.ActualSales   = vm.WeekLabels.Select(_ => rnd.Next(5,25)).ToList();
            vm.ForecastSales = vm.WeekLabels.Select(_ => rnd.Next(5,25)).ToList();

            return View(vm);
        }
    }
}
