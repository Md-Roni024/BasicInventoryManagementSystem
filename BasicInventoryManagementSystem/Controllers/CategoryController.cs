using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using System.Linq;
using BasicInventoryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace BasicInventoryManagementSystem.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedDate = DateTime.UtcNow; // Set CreatedDate
                category.UpdatedDate = DateTime.UtcNow; // Set UpdatedDate initially
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = _context.Categories.Find(category.Id);
                if (existingCategory != null)
                {
                    existingCategory.CategoryName = category.CategoryName;
                    // Do not change CreatedDate
                    existingCategory.UpdatedDate = DateTime.UtcNow; // Update the UpdatedDate

                    _context.Update(existingCategory);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }


        //Delete
        // GET: Category/Delete/5
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { deleted = true });
        }
    }
}
