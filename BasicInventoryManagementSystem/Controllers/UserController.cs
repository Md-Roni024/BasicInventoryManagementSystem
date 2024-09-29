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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.Tasks;

namespace BasicInventoryManagementSystem.Controllers
{

    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "SuperAdmin")]
        //public async Task<IActionResult> Index()
        //{

        //    var users = _userManager.Users.ToList();
        //    var userRoles = new Dictionary<string, string>();

        //    foreach (var user in users)
        //    {
        //        var roles = await _userManager.GetRolesAsync(user);
        //        userRoles[user.Id] = roles.Count > 0 ? string.Join(", ", roles) : "No roles assigned";
        //    }

        //    ViewBag.UserRoles = userRoles;
        //    return View(users);
        //}
        public async Task<IActionResult> Index(string? search)
        {
            // Retrieve all users
            var users = _userManager.Users.AsQueryable(); // Make it a queryable to allow filtering

            // Apply search filtering if search term is provided
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Name.Contains(search) || u.Email.Contains(search)); // Filter by Name or Email
            }

            // Convert to list for further processing
            var userList = await users.ToListAsync();

            var userRoles = new Dictionary<string, string>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.Count > 0 ? string.Join(", ", roles) : "No roles assigned";
            }

            ViewBag.UserRoles = userRoles;
            return View(userList); // Return the filtered user list
        }


        [HttpGet]
        public IActionResult Register()
        {
            ViewData["IsRegisterPage"] = true;
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
            ViewData["IsLoginPage"] = true;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); 
            return RedirectToAction("Login", "User"); 
        }



        [Authorize(Roles = "SuperAdmin")]
        // GET: User/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/DeleteConfirmed
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    // Redirect with query parameter to show toast notification
                    return RedirectToAction("Index", new { deleted = true });
                }
                // Handle errors if any
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user); // Get user's current roles
            var allRoles = _roleManager.Roles.ToList(); // Get all available roles

            var model = new AssignRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = allRoles.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name,
                    Selected = userRoles.Contains(role.Name)
                }).ToList()
            };

            return View(model);
        }

        // POST: Users/AssignRole/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Remove current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove current roles.");
                return View(model);
            }

            // Add new role
            var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
            if (!addRoleResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add the new role.");
                return View(model);
            }

            return RedirectToAction(nameof(Index)); // Redirect back to the user list
        }

    }
}