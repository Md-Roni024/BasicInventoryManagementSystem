using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using System.Linq;
using BasicInventoryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BasicInventoryManagementSystem.ViewModel;

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
        public IActionResult Index(string? search, int page = 1)
        {
            int pageSize = 10;
            var categories = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(p => p.CategoryName.Contains(search));
            }

            var paginatedCategories = categories
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = categories.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var viewModel = new CategoryIndexViewModel
            {
                Categories = paginatedCategories,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }


        [Authorize(Roles = "SuperAdmin, Admin")]
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
                try
                {
                    category.CreatedDate = DateTime.UtcNow;
                    category.UpdatedDate = DateTime.UtcNow;
                    _context.Categories.Add(category);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving changes to the database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
                }
            }
            return View(category);
        }


        [Authorize(Roles = "SuperAdmin, Admin")]
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = _context.Categories.Find(category.Id);
                    if (existingCategory != null)
                    {
                        existingCategory.CategoryName = category.CategoryName;
                        existingCategory.UpdatedDate = DateTime.UtcNow;

                        _context.Update(existingCategory);
                        _context.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving changes to the database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
                }
            }
            return View(category);
        }



        [Authorize(Roles = "SuperAdmin, Admin")]
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
