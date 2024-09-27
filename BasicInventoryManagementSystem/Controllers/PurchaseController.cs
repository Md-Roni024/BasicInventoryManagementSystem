using BasicInventoryManagementSystem.Data;
using BasicInventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BasicInventoryManagementSystem.Controllers
{
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
            var purchases = await _context.Purchases.Include(p => p.Product).ToListAsync();
            return View(purchases);
        }

        // GET: Purchase/Create
        public IActionResult Create()
        {
            PrepareProductDropdown();
            return View();
        }

        // POST: Purchase/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(purchase.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", "Selected product does not exist.");
                    PrepareProductDropdown();
                    return View(purchase);
                }

                purchase.CreatedDate = DateTime.UtcNow;
                _context.Purchases.Add(purchase);
                product.Quantity += purchase.Quantity; // Add stock on purchase
                _context.Update(product);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PrepareProductDropdown();
            return View(purchase);
        }

        // GET: Purchase/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            PrepareProductDropdown(purchase.ProductId);
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
                    var originalPurchase = await _context.Purchases.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (originalPurchase == null)
                    {
                        return NotFound();
                    }

                    var product = await _context.Products.FindAsync(purchase.ProductId);
                    if (product == null)
                    {
                        ModelState.AddModelError("", "Selected product does not exist.");
                        PrepareProductDropdown(purchase.ProductId);
                        return View(purchase);
                    }

                    int quantityDifference = purchase.Quantity - originalPurchase.Quantity;

                    product.Quantity += quantityDifference; // Adjust stock based on the quantity difference
                    _context.Update(product);

                    purchase.UpdatedDate = DateTime.UtcNow;
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
                        ModelState.AddModelError("", "Concurrency error. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error occurred while updating the purchase: {ex.Message}");
                }

                return RedirectToAction(nameof(Index));
            }

            PrepareProductDropdown(purchase.ProductId);
            return View(purchase);
        }

        // GET: Purchase/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases.Include(p => p.Product).FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchase/Delete/5
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
                    product.Quantity -= purchase.Quantity; // Reduce stock on purchase deletion
                    _context.Update(product);
                }

                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }

        private void PrepareProductDropdown(int? selectedProductId = null)
        {
            var products = _context.Products.ToList();
            ViewData["ProductId"] = new SelectList(products, "Id", "Name", selectedProductId);
        }
    }
}
