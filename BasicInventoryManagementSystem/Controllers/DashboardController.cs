using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BasicInventoryManagementSystem.Models;
using BasicInventoryManagementSystem.Data;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        // Total Sales for the current month
        var totalSales = await _context.Sales
            .Where(s => s.CreatedDate.Month == currentMonth && s.CreatedDate.Year == currentYear)
            .SumAsync(s => s.Price * s.Quantity);

        // Total Purchases for the current month
        var totalPurchases = await _context.Purchases
            .Where(p => p.CreatedDate.Month == currentMonth && p.CreatedDate.Year == currentYear)
            .SumAsync(p => p.Price * p.Quantity);

        // Total Quantity Sold
        var totalQuantitySold = await _context.Sales
            .Where(s => s.CreatedDate.Month == currentMonth && s.CreatedDate.Year == currentYear)
            .SumAsync(s => s.Quantity);

        // Total Quantity Purchased
        var totalQuantityPurchased = await _context.Purchases
            .Where(p => p.CreatedDate.Month == currentMonth && p.CreatedDate.Year == currentYear)
            .SumAsync(p => p.Quantity);

        // Sales grouped by day of the current month
        var salesData = await _context.Sales
            .Where(s => s.CreatedDate.Month == currentMonth && s.CreatedDate.Year == currentYear)
            .GroupBy(s => s.CreatedDate.Day)
            .Select(g => new { Day = g.Key, TotalSales = g.Sum(s => s.Price * s.Quantity) })
            .ToListAsync();

        // Purchases grouped by day of the current month
        var purchaseData = await _context.Purchases
            .Where(p => p.CreatedDate.Month == currentMonth && p.CreatedDate.Year == currentYear)
            .GroupBy(p => p.CreatedDate.Day)
            .Select(g => new { Day = g.Key, TotalPurchases = g.Sum(p => p.Price * p.Quantity) })
            .ToListAsync();

        // Pass the data to the view
        ViewBag.TotalSales = totalSales;
        ViewBag.TotalPurchases = totalPurchases;
        ViewBag.TotalQuantitySold = totalQuantitySold;
        ViewBag.TotalQuantityPurchased = totalQuantityPurchased;
        ViewBag.SalesData = salesData;
        ViewBag.PurchaseData = purchaseData;

        return View();
    }
}
