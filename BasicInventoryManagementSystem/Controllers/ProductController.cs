using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using BasicInventoryManagementSystem.ViewModel;
using BasicInventoryManagementSystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search) || p.CategoryName.Contains(search));
            }
            var totalProducts = await products.CountAsync();

            var productsPaged = await products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ProductIndexViewModel
            {
                Products = productsPaged,
                TotalProducts = totalProducts,
                CurrentPage = page,
                PageSize = pageSize,
                SearchTerm = search
            };

            return View(viewModel);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: Product/Create
        public IActionResult Create()
        {
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
                try
                {
                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product: " + innerExceptionMessage);
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product: " + errorMessage);
                }
            }

            var categories = _context.Categories.ToList();
            ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName");

            return View(product);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

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
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product: " + innerExceptionMessage);
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product: " + errorMessage);
                }
            }

            var categories = _context.Categories.ToList();
            ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName", product.CategoryName);
            return View(product);
        }



        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
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
