using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BasicInventoryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace BasicInventoryManagementSystem.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchase
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Product) // Include the related Product
                .ToListAsync();
            return View(purchases);
        }

        // GET: Purchase/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList(); // Get products for dropdown
            ViewBag.Categories = _context.Categories.ToList(); // Get categories for dropdown
            return View();
        }

        // POST: Purchase/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                // Add the purchase
                _context.Add(purchase);
                var product = await _context.Products.FindAsync(purchase.ProductId);

                // Update product quantity
                if (product != null)
                {
                    product.Quantity += purchase.Quantity; // Add quantity
                    _context.Update(product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBag in case of validation failure
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(purchase);
        }

        // GET: Purchase/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Product) // Include related Product details
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
                    product.Quantity -= purchase.Quantity; // Subtract quantity
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
                .Include(p => p.Product) // Include related Product details
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _context.Products.ToListAsync(); // Get products for dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync(); // Get categories for dropdown
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
                    // Find the existing purchase to get the old quantity
                    var existingPurchase = await _context.Purchases
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    // Update the product's quantity
                    var product = await _context.Products.FindAsync(purchase.ProductId);
                    if (product != null)
                    {
                        // Adjust the product quantity based on the new purchase quantity
                        if (existingPurchase.ProductId != purchase.ProductId)
                        {
                            // Subtract quantity from old product
                            var oldProduct = await _context.Products.FindAsync(existingPurchase.ProductId);
                            if (oldProduct != null)
                            {
                                oldProduct.Quantity -= existingPurchase.Quantity;
                                _context.Update(oldProduct);
                            }

                            // Add quantity to new product
                            product.Quantity += purchase.Quantity;
                        }
                        else
                        {
                            // If the product hasn't changed, just update the quantity
                            product.Quantity += purchase.Quantity - existingPurchase.Quantity;
                        }

                        _context.Update(product);
                    }

                    // Update the purchase
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

            // Repopulate ViewBag in case of validation failure
            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(purchase);
        }

        // Check if Purchase exists
        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
