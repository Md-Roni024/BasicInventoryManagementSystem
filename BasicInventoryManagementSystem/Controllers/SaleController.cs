using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using BasicInventoryManagementSystem.Data;
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
        public IActionResult Index()
        {
            var sales = _context.Sales.ToList();
            return View(sales);
        }

        // GET: Sale/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList(); // Pass products to the view
            return View();
        }

        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale)
        {
            if (ModelState.IsValid)
            {
                sale.CreatedDate = DateTime.UtcNow;
                sale.UpdatedDate = DateTime.UtcNow;
                await _context.Sales.AddAsync(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList(); // Pass products to the view
            return View(sale);
        }

        // GET: Sale/Edit/5
        public IActionResult Edit(int id)
        {
            var sale = _context.Sales.Find(id);
            if (sale == null)
            {
                return NotFound();
            }
            ViewBag.Products = _context.Products.ToList(); // Pass products to the view
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
                sale.UpdatedDate = DateTime.UtcNow; // Update the date on edit
                _context.Update(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList(); // Pass products to the view
            return View(sale);
        }

        // GET: Sale/Delete/5
        public IActionResult Delete(int id)
        {
            var sale = _context.Sales.Find(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        // POST: Sale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
