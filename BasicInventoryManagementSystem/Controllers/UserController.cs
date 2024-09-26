using Microsoft.AspNetCore.Mvc;
using BasicInventoryManagementSystem.Models;
using BasicInventoryManagementSystem.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace BasicInventoryManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                Log.Information($"User registered: {user.Email}");
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                Log.Information($"User logged in: {user.Email}");
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }
    }
}
