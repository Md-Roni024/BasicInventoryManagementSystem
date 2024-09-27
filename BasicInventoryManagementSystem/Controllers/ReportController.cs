using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicInventoryManagementSystem.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using BasicInventoryManagementSystem.Data;

namespace BasicInventoryManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Purchase()
        {
            var firstDayOfCurrentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

            var purchases = _context.Purchases
                .Where(p => p.CreatedDate >= firstDayOfCurrentMonth && p.CreatedDate <= lastDayOfCurrentMonth)
                .Include(p => p.Product)
                .ToList();

            var totalPurchases = purchases.Sum(p => p.Price * p.Quantity);
            ViewBag.TotalPurchases = totalPurchases;

            return View(purchases);
        }

        // New action for downloading the CSV report
        public IActionResult DownloadPurchaseReport()
        {
            var firstDayOfCurrentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

            var purchases = _context.Purchases
                .Where(p => p.CreatedDate >= firstDayOfCurrentMonth && p.CreatedDate <= lastDayOfCurrentMonth)
                .Include(p => p.Product)
                .ToList();

            var totalPurchases = purchases.Sum(p => p.Price * p.Quantity);

            // Create CSV file content
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Product,Quantity,Price,Supplier,Created Date");
            foreach (var purchase in purchases)
            {
                csvBuilder.AppendLine($"{purchase.Product?.Name ?? "N/A"},{purchase.Quantity},{purchase.Price},{purchase.Suppliere},{purchase.CreatedDate.ToString("g")}");
            }
            csvBuilder.AppendLine($"Total Purchases,,{totalPurchases},,");

            // Generate file name with current month and year
            var currentMonthName = DateTime.UtcNow.ToString("MMMM");
            var currentYear = DateTime.UtcNow.Year;
            var fileName = $"PurchaseReport-{currentMonthName}-{currentYear}.csv";
            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(bytes, "text/csv", fileName);
        }
    }
}
