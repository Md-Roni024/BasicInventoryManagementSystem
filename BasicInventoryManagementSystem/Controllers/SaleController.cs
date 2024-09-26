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
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sale
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales.Include(s => s.Product).ToListAsync();
            return View(sales);
        }

        // GET: Sale/Create
        public IActionResult Create()
        {
            PrepareProductDropdown();
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
                if (product == null)
                {
                    ModelState.AddModelError("", "Selected product does not exist.");
                    PrepareProductDropdown();
                    return View(sale);
                }

                if (product.Quantity < sale.Quantity)
                {
                    ModelState.AddModelError("", "Not enough stock for this sale.");
                    PrepareProductDropdown();
                    return View(sale);
                }

                sale.CreatedDate = DateTime.UtcNow;
                _context.Sales.Add(sale);
                product.Quantity -= sale.Quantity;
                _context.Update(product);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PrepareProductDropdown();
            return View(sale);
        }

        // GET: Sale/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            PrepareProductDropdown(sale.ProductId);
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
                    var originalSale = await _context.Sales.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    if (originalSale == null)
                    {
                        return NotFound();
                    }

                    var product = await _context.Products.FindAsync(sale.ProductId);
                    if (product == null)
                    {
                        ModelState.AddModelError("", "Selected product does not exist.");
                        PrepareProductDropdown(sale.ProductId);
                        return View(sale);
                    }

                    int quantityDifference = sale.Quantity - originalSale.Quantity;

                    if (product.Quantity < quantityDifference)
                    {
                        ModelState.AddModelError("", "Not enough stock for this sale update.");
                        PrepareProductDropdown(sale.ProductId);
                        return View(sale);
                    }

                    sale.UpdatedDate = DateTime.UtcNow;
                    _context.Update(sale);

                    product.Quantity -= quantityDifference;
                    _context.Update(product);

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
                        ModelState.AddModelError("", "Concurrency error. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error occurred while updating the sale: {ex.Message}");
                }

                return RedirectToAction(nameof(Index));
            }

            PrepareProductDropdown(sale.ProductId);
            return View(sale);
        }

        // GET: Sale/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.Include(s => s.Product).FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sale/Delete/5
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
                    _context.Update(product);
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }

        private void PrepareProductDropdown(int? selectedProductId = null)
        {
            var products = _context.Products.ToList();
            ViewData["ProductId"] = new SelectList(products, "Id", "Name", selectedProductId);
        }
    }
}
