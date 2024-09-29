using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BasicInventoryManagementSystem.Data;
using BasicInventoryManagementSystem.ViewModel;

namespace BasicInventoryManagementSystem.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: Sale

        //public IActionResult Index(string? search)
        //{
        //    // Query the sales table and include the related Product
        //    var sales = _context.Sales
        //        .Include(s => s.Product) // Include the related Product
        //        .AsQueryable();
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        sales = sales.Where(s => s.Product.Name.Contains(search) ||
        //                                 s.CategoryName.Contains(search));
        //    }

        //    return View(sales.ToList());
        //}

        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            // Query the sales table and include the related Product
            var salesQuery = _context.Sales
                .Include(s => s.Product) // Include the related Product
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                salesQuery = salesQuery.Where(s => s.Product.Name.Contains(search) ||
                                                   s.CategoryName.Contains(search));
            }

            // Pagination logic
            var totalSalesCount = salesQuery.Count();
            var sales = salesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create ViewModel to pass data to the view
            var viewModel = new SaleIndexViewModel
            {
                Sales = sales,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSalesCount / (double)pageSize)
            };

            return View(viewModel);
        }


        // GET: Sale/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList(); // Get products for dropdown
            ViewBag.Categories = _context.Categories.ToList(); // Get categories for dropdown
            return View();
        }

        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale)
        {
            if (ModelState.IsValid)
            {
                // Check if the sale quantity is less than or equal to the product's available quantity
                var product = await _context.Products.FindAsync(sale.ProductId);
                if (product == null || sale.Quantity > product.Quantity)
                {
                    ModelState.AddModelError("Quantity", "Sale quantity must be less than or equal to the available product quantity.");
                    ViewBag.Products = _context.Products.ToList();
                    ViewBag.Categories = _context.Categories.ToList();
                    return View(sale);
                }

                // Add the sale
                _context.Add(sale);
                product.Quantity -= sale.Quantity; // Decrease the product quantity
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBag in case of validation failure
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(sale);
        }

        // GET: Sale/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Product) // Include related Product details
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sale/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                var product = await _context.Products.FindAsync(sale.ProductId);
                if (product != null)
                {
                    product.Quantity += sale.Quantity; // Increase the product quantity
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Sale/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Product) // Include related Product details
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _context.Products.ToListAsync(); // Get products for dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync(); // Get categories for dropdown
            return View(sale);
        }

        // POST: Sale/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sale sale)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing sale
                    var existingSale = await _context.Sales
                        .Include(s => s.Product) // Include the related Product
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (existingSale == null)
                    {
                        return NotFound();
                    }

                    // Check if the new quantity is valid compared to the product's quantity
                    var product = await _context.Products.FindAsync(sale.ProductId);
                    if (product == null || sale.Quantity > product.Quantity + existingSale.Quantity)
                    {
                        ModelState.AddModelError("Quantity", "Sale quantity must be less than or equal to the available product quantity.");
                        ViewBag.Products = await _context.Products.ToListAsync();
                        ViewBag.Categories = await _context.Categories.ToListAsync();
                        return View(sale);
                    }

                    // Update product quantity
                    product.Quantity = product.Quantity + existingSale.Quantity - sale.Quantity; // Adjust product quantity

                    // Update existing sale properties
                    existingSale.ProductId = sale.ProductId; // Update product id
                    existingSale.Quantity = sale.Quantity; // Update quantity
                    // Update other properties as needed
                    // existingSale.OtherProperty = sale.OtherProperty;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBag in case of validation failure
            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(sale);
        }

        // Check if Sale exists
        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
