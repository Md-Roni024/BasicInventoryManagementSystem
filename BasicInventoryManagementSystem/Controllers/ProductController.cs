//using Microsoft.AspNetCore.Mvc;
//using BasicInventoryManagementSystem.Models;
//using BasicInventoryManagementSystem.Data;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace BasicInventoryManagementSystem.Controllers
//{
//    [Authorize]
//    public class ProductController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public ProductController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Product
//        public IActionResult Index()
//        {
//            var products = _context.Products.ToList();
//            return View(products);
//        }


//        // POST: Product/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Product product)
//        {
//            if (ModelState.IsValid)
//            {
//                // Assuming product has a CategoryId property
//                await _context.Products.AddAsync(product);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }

//            // Repopulate categories in case of an error
//            var categories = _context.Categories.ToList();
//            ViewBag.CategoryId = new SelectList(categories, "Id", "CategoryName");

//            return View(product);
//        }


//        // GET: Product/Edit/5
//        public IActionResult Edit(int id)
//        {
//            var product = _context.Products.Find(id);
//            if (product == null)
//            {
//                return NotFound();
//            }
//            return View(product);
//        }

//        // POST: Product/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Product product)
//        {
//            if (id != product.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                _context.Update(product);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(product);
//        }

//        // GET: Product/Delete/5
//        public IActionResult Delete(int id)
//        {
//            var product = _context.Products.Find(id);
//            if (product == null)
//            {
//                return NotFound();
//            }
//            return View(product);
//        }

//        // POST: Product/DeleteConfirmed
//        [HttpPost, ActionName("DeleteConfirmed")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var product = await _context.Products.FindAsync(id);
//            if (product != null)
//            {
//                _context.Products.Remove(product);
//                await _context.SaveChangesAsync();
//            }
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}



using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
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
        //public IActionResult Index()
        //{
        //    var products = _context.Products.ToList();
        //    return View(products);
        //}

        public IActionResult Index(string? search)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Filter by name or category
                products = products.Where(p => p.Name.Contains(search) ||
                                                p.CategoryName.Contains(search));
            }

            return View(products.ToList());
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _context.Products.AddAsync(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Repopulate categories if the form submission fails
        //    var categories = _context.Categories.ToList();
        //    ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName");
        //    return View(product);
        //}

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
                    // Log the exception details
                    var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product: " + innerExceptionMessage);
                }
                catch (Exception ex)
                {
                    // Log the exception details for any other unexpected exceptions
                    var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product: " + errorMessage);
                }
            }

            // Repopulate categories in case of an error
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Repopulate categories if the form submission fails
        //    var categories = _context.Categories.ToList();
        //    ViewBag.CategoryName = new SelectList(categories, "CategoryName", "CategoryName", product.CategoryName);
        //    return View(product);
        //}
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
                    // Log the exception details
                    var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product: " + innerExceptionMessage);
                }
                catch (Exception ex)
                {
                    // Log the exception details for any other unexpected exceptions
                    var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product: " + errorMessage);
                }
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
