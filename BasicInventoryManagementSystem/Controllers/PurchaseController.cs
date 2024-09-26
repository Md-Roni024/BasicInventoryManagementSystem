// Controllers/PurchaseController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicInventoryManagementSystem.Models;
using System.Threading.Tasks;
using BasicInventoryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BasicInventoryManagementSystem.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases.Include(p => p.Product).ToListAsync();
            return View(purchases);
        }

        public IActionResult Create()
        {
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(purchase.ProductId);
                if (product != null)
                {
                    product.Quantity += purchase.Quantity; // Increase product count
                    _context.Products.Update(product);
                }

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", purchase.ProductId);
            return View(purchase);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", purchase.ProductId);
            return View(purchase);
        }

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
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", purchase.ProductId);
            return View(purchase);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _context.Purchases.Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }
            return View(purchase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                var product = await _context.Products.FindAsync(purchase.ProductId);
                if (product != null)
                {
                    product.Quantity -= purchase.Quantity; // Decrease product count
                    _context.Products.Update(product);
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
    }
}
