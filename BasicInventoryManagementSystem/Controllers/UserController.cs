//using Microsoft.AspNetCore.Mvc;
//using BasicInventoryManagementSystem.Models;
//using BasicInventoryManagementSystem.Data;
//using System.Linq;
//using Microsoft.AspNetCore.Http;
//using Serilog;

//namespace BasicInventoryManagementSystem.Controllers
//{
//    public class UserController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public UserController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Register()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Register(User user)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Users.Add(user);
//                _context.SaveChanges();
//                Log.Information($"User registered: {user.Email}");
//                return RedirectToAction("Login");
//            }
//            return View(user);
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login(string email, string password)
//        {
//            var user = _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
//            if (user != null)
//            {
//                Log.Information($"User logged in: {user.Email}");
//                return RedirectToAction("Index", "Home");
//            }
//            ModelState.AddModelError("", "Invalid login attempt.");
//            return View();
//        }
//    }
//}


//using Microsoft.AspNetCore.Mvc;
//using BasicInventoryManagementSystem.Models;
//using BasicInventoryManagementSystem.Data;
//using Microsoft.AspNetCore.Identity;
//using System.Threading.Tasks;
//using Serilog;
//using BasicInventoryManagementSystem.ViewModel;
//using BasicInventoryManagementSystem.ViewModel.UserViewModel;

//namespace BasicInventoryManagementSystem.Controllers
//{
//    public class UserController : Controller
//    {
//        private readonly UserManager<User> _userManager;
//        private readonly SignInManager<User> _signInManager;
//        private readonly ApplicationDbContext _context;

//        public UserController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
//        {
//            _context = context;
//            _userManager = userManager;
//            _signInManager = signInManager;
//        }

//        [HttpGet]
//        public IActionResult Register()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Register(UserViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name };
//                var result = await _userManager.CreateAsync(user, model.Password);
//                if (result.Succeeded)
//                {
//                    Log.Information($"User registered: {model.Email}");
//                    return RedirectToAction("Login");
//                }
//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            return View(model);
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Login(LoginViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
//                if (result.Succeeded)
//                {
//                    Log.Information($"User logged in: {model.Email}");
//                    return RedirectToAction("Index", "Home");
//                }
//                ModelState.AddModelError("", "Invalid login attempt.");
//            }
//            return View(model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Logout()
//        {
//            await _signInManager.SignOutAsync();
//            return RedirectToAction("Login");
//        }
//    }
//}

using BasicInventoryManagementSystem.Models;
using BasicInventoryManagementSystem.ViewModel;
using BasicInventoryManagementSystem.ViewModel.UserViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace BasicInventoryManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            Log.Information("Register action started");
            if (ModelState.IsValid)
            {
                Log.Information($"Attempting to create user: {model.Email}");
                var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    Log.Information($"User created successfully: {user.Email}");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    Log.Warning($"Failed to create user: {user.Email}");
                    foreach (var error in result.Errors)
                    {
                        Log.Warning($"Error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                Log.Warning("ModelState is not valid");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        Log.Warning($"Errors for {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Product");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }
    }
}