using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicInventoryManagementSystem.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using BasicInventoryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace BasicInventoryManagementSystem.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
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

        public IActionResult DownloadPurchaseReport()
        {
            var firstDayOfCurrentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

            var purchases = _context.Purchases
                .Where(p => p.CreatedDate >= firstDayOfCurrentMonth && p.CreatedDate <= lastDayOfCurrentMonth)
                .Include(p => p.Product)
                .ToList();

            var totalPurchases = purchases.Sum(p => p.Price * p.Quantity);

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Product,Quantity,Price,Supplier,Created Date");
            foreach (var purchase in purchases)
            {
                csvBuilder.AppendLine($"{purchase.Product?.Name ?? "N/A"},{purchase.Quantity},{purchase.Price},{purchase.Supplier},{purchase.CreatedDate.ToString("g")}");
            }
            csvBuilder.AppendLine($"Total Purchases,,{totalPurchases},,");

            var currentMonthName = DateTime.UtcNow.ToString("MMMM");
            var currentYear = DateTime.UtcNow.Year;
            var fileName = $"PurchaseReport-{currentMonthName}-{currentYear}.csv";
            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(bytes, "text/csv", fileName);
        }

        // New action for Sale Report
        public IActionResult Sale()
        {
            var firstDayOfCurrentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

            var sales = _context.Sales
                .Where(s => s.CreatedDate >= firstDayOfCurrentMonth && s.CreatedDate <= lastDayOfCurrentMonth)
                .Include(s => s.Product)
                .ToList();

            var totalSales = sales.Sum(s => s.Price * s.Quantity);
            ViewBag.TotalSales = totalSales;

            return View(sales);
        }

        // New action for downloading the Sale CSV report
        public IActionResult DownloadSaleReport()
        {
            var firstDayOfCurrentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

            var sales = _context.Sales
                .Where(s => s.CreatedDate >= firstDayOfCurrentMonth && s.CreatedDate <= lastDayOfCurrentMonth)
                .Include(s => s.Product)
                .ToList();

            var totalSales = sales.Sum(s => s.Price * s.Quantity);

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Product,Quantity,Price,Seller,Created Date");
            foreach (var sale in sales)
            {
                csvBuilder.AppendLine($"{sale.Product?.Name ?? "N/A"},{sale.Quantity},{sale.Price},{sale.Seller},{sale.CreatedDate.ToString("g")}");
            }
            csvBuilder.AppendLine($"Total Sales,,{totalSales},,");

            var currentMonthName = DateTime.UtcNow.ToString("MMMM");
            var currentYear = DateTime.UtcNow.Year;
            var fileName = $"SaleReport-{currentMonthName}-{currentYear}.csv";
            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(bytes, "text/csv", fileName);
        }

        // New action for Product Report
        public IActionResult InventoryStatus()
        {
            var products = _context.Products.ToList(); // Get all products from the database
            return View(products); // Pass the products to the view
        }

        // New action for downloading the Product CSV report
        public IActionResult DownloadProductReport()
        {
            var products = _context.Products.ToList();

            // Create CSV file content
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Product Name,Quantity,Price,Created Date");
            foreach (var product in products)
            {
                csvBuilder.AppendLine($"{product.Name},{product.Quantity},{product.Price},{product.CreatedDate.ToString("g")}");
            }

            var currentMonthName = DateTime.UtcNow.ToString("MMMM");
            var currentYear = DateTime.UtcNow.Year;
            var fileName = $"ProductReport-{currentMonthName}-{currentYear}.csv";
            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(bytes, "text/csv", fileName);
        }
    }
}
