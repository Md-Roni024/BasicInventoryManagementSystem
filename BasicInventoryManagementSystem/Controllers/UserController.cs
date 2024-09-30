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
        public async Task<IActionResult> Index(string? search)
        {
            var users = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
            }
            var userList = await users.ToListAsync();
            var userRoles = new Dictionary<string, string>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.Count > 0 ? string.Join(", ", roles) : "No roles assigned";
            }
            ViewBag.UserRoles = userRoles;
            return View(userList);
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
                    return RedirectToAction("Index", "Home");
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
                    return RedirectToAction("Index", new { deleted = true });
                }
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

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

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
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove current roles.");
                return View(model);
            }
            var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
            if (!addRoleResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add the new role.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}