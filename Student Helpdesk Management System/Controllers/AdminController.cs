using HelpdeskModel.ViewModels;
using HelpdeskRepository.Data;
using HelpdeskService.Services;          
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly AppDbContext _context;

        public AdminController(IStaffService staffService, AppDbContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DashBoard()
        {
            return View();
        }

        // CREATE STAFF
        [HttpGet]
        public IActionResult CreateStaff()
        {
            ViewBag.Departments = _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(StaffRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await _staffService.CreateStaffAsync(model);

            if (!ok)
            {
                ModelState.AddModelError("", "Failed to create staff. Please try again.");
                return View(model);
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}
