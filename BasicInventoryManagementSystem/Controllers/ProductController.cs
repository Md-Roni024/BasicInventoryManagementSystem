using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using BasicInventoryManagementSystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BasicInventoryManagementSystem.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: Product/Create
        public IActionResult Create()
        {
            // Fetch all categories for dropdown
            var categories = _context.Categories.ToList();
            ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate categories if the form submission fails
            var categories = _context.Categories.ToList();
            ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName");
            return View(product);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: Product/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Fetch all categories for dropdown
            var categories = _context.Categories.ToList();
            ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName", product.CategoryName);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate categories if the form submission fails
            var categories = _context.Categories.ToList();
            ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName", product.CategoryName);
            return View(product);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: Product/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
