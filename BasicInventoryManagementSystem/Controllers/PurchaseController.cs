using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BasicInventoryManagementSystem.Data;
using BasicInventoryManagementSystem.ViewModel;

namespace BasicInventoryManagementSystem.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }


        //Get all purchase

        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var purchases = _context.Purchases
                .Include(p => p.Product) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                purchases = purchases.Where(s => s.Product.Name.Contains(search) ||
                                             s.CategoryName.Contains(search));
            }
            var totalPurchases = purchases.Count();
            var totalPages = (int)Math.Ceiling(totalPurchases / (double)pageSize);
            var purchasesToDisplay = purchases.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new PurchaseIndexViewModel
            {
                Purchases = purchasesToDisplay,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchQuery = search
            };

            return View(viewModel);
        }


        // GET: Purchase/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList(); 
            ViewBag.Categories = _context.Categories.ToList(); 
            return View();
        }

        // POST: Purchase/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchase);
                var product = await _context.Products.FindAsync(purchase.ProductId);
                if (product != null)
                {
                    product.Quantity += purchase.Quantity;
                    _context.Update(product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(purchase);
        }



        // GET: Purchase/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Product) 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchase/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                var product = await _context.Products.FindAsync(purchase.ProductId);
                if (product != null)
                {
                    product.Quantity -= purchase.Quantity;
                    _context.Update(product);
                }
                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



        // GET: Purchase/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _context.Products.ToListAsync(); 
            ViewBag.Categories = await _context.Categories.ToListAsync(); 
            return View(purchase);
        }


        // POST: Purchase/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
         var existingPurchase = await _context.Purchases
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    var product = await _context.Products.FindAsync(purchase.ProductId);
                    if (product != null)
                    {
                        if (existingPurchase.ProductId != purchase.ProductId)
                        {
                            var oldProduct = await _context.Products.FindAsync(existingPurchase.ProductId);
                            if (oldProduct != null)
                            {
                                oldProduct.Quantity -= existingPurchase.Quantity;
                                _context.Update(oldProduct);
                            }
                            product.Quantity += purchase.Quantity;
                        }
                        else
                        {
                            product.Quantity += purchase.Quantity - existingPurchase.Quantity;
                        }

                        _context.Update(product);
                    }
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.Id))
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
            return View(purchase);
        }
        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
