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
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var salesQuery = _context.Sales
                .Include(s => s.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                salesQuery = salesQuery.Where(s => s.Product.Name.Contains(search) ||
                                                   s.CategoryName.Contains(search));
            }
            var totalSalesCount = salesQuery.Count();
            var sales = salesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
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
            ViewBag.Products = _context.Products.ToList(); 
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(sale.ProductId);
                if (product == null || sale.Quantity > product.Quantity)
                {
                    ModelState.AddModelError("Quantity", "Sale quantity must be less than or equal to the available product quantity.");
                    ViewBag.Products = _context.Products.ToList();
                    ViewBag.Categories = _context.Categories.ToList();
                    return View(sale);
                }
                _context.Add(sale);
                product.Quantity -= sale.Quantity;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(sale);
        }


        // GET: Sale/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Product)
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
                    product.Quantity += sale.Quantity;
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
                .Include(s => s.Product) 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _context.Products.ToListAsync(); 
            ViewBag.Categories = await _context.Categories.ToListAsync();
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
                    var existingSale = await _context.Sales
                        .Include(s => s.Product)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (existingSale == null)
                    {
                        return NotFound();
                    }
                    var product = await _context.Products.FindAsync(sale.ProductId);
                    if (product == null || sale.Quantity > product.Quantity + existingSale.Quantity)
                    {
                        ModelState.AddModelError("Quantity", "Sale quantity must be less than or equal to the available product quantity.");
                        ViewBag.Products = await _context.Products.ToListAsync();
                        ViewBag.Categories = await _context.Categories.ToListAsync();
                        return View(sale);
                    }
                    product.Quantity = product.Quantity + existingSale.Quantity - sale.Quantity;
                    existingSale.ProductId = sale.ProductId;
                    existingSale.Quantity = sale.Quantity;
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
            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(sale);
        }
        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
