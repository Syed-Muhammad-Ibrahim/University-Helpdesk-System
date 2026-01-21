using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskRepository.Data;
using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace UserRoles.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly AppDbContext context;
        private readonly IStudentService studentService;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            AppDbContext context,
            IStudentService studentService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
            this.studentService = studentService;
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
            if (!ModelState.IsValid)
                return View(model);

            var result = await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Redirect based on user’s stored roles
            if (await userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("DashBoard", "Admin");

            if (await userManager.IsInRoleAsync(user, "Staff"))
                return RedirectToAction("Dashboard", "Staff");

            if (await userManager.IsInRoleAsync(user, "Student"))
                return RedirectToAction("Dashboard", "Student");

            // Fallback
            return RedirectToAction("Index", "Home");
        }


        //Register actions
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                FullName = model.Name,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = "User" });
                }

                await userManager.AddToRoleAsync(user, "User");
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // STUDENT REGISTER
        [HttpGet]

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterStudent()
        {
            ViewBag.Departments = context.Departments
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                .ToList();

            return View("RegisterStudent");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(StudentRegisterViewModel model)
        {
            ViewBag.Departments = context.Departments
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                .ToList();

            if (!ModelState.IsValid)
                return View("RegisterStudent", model);

            long? createdById = null;
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserIdString) && long.TryParse(currentUserIdString, out var currentUserId))
                createdById = currentUserId;

            var result = await studentService.CreateStudentAsync(model, createdById);

            if (!result.Succeeded)
            {
                foreach (var kv in result.Errors)
                    foreach (var msg in kv.Value)
                        ModelState.AddModelError(kv.Key, msg);

                return View("RegisterStudent", model);
            }

            var newUser = await userManager.FindByEmailAsync(model.Email);
            if (newUser != null)
                await signInManager.SignInAsync(newUser, isPersistent: false);

            return RedirectToAction("Dashboard", "Student");
        }



        // ADMIN REGISTER (usually protected)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View("RegisterAdmin");
        }

        [Authorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("RegisterAdmin", model);

            var user = new ApplicationUser
            {
                FullName = model.Name,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });

                await userManager.AddToRoleAsync(user, "Admin");
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("RegisterAdmin", model);
        }


        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
        }

        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            var result = await userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddPasswordAsync(user, model.NewPassword);
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
