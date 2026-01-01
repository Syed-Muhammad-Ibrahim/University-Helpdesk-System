using HelpdeskModel.BusinessRules;
using HelpdeskModel.ViewModels;
using HelpdeskModel.ViewModels.UpdateViewModels;
using HelpdeskRepository.Data;
using HelpdeskService.Services;          
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly IStudentService _studentService;
        private readonly AppDbContext _context;

        public AdminController(IStaffService staffService,IStudentService studentService, AppDbContext context)
        {
            _staffService = staffService;
            _studentService = studentService;
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

        // Create Staff
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
            {
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();

                return View(model);
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long? currentUserId = null;
            if (!string.IsNullOrEmpty(userIdString) && long.TryParse(userIdString, out var parsed))
                currentUserId = parsed;

            var ok = await _staffService.CreateStaffAsync(model, currentUserId);

            if (!ok)
            {
                ModelState.AddModelError("", "Failed to create staff. Please try again.");

                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();

                return View(model);
            }

            return RedirectToAction("DashBoard", "Admin");
        }

        // Staff List
        public async Task<IActionResult> StaffList()
        {
            var staffs = await _staffService.GetAllStaffAsync();
            return View(staffs);
        }


        // Update Staff
        [HttpGet]
        public async Task<IActionResult> EditStaff(long id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
                return NotFound();

            var model = new StaffUpdateViewModel
            {
                Id = staff.Id,
                Name = staff.Name,
                Address = staff.Address,
                Phone = staff.Phone,
                DepartmentId = staff.Department.Id,
                Status = staff.Status
            };

            ViewBag.Departments = _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(StaffUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var currentUserId))
            {
                ModelState.AddModelError("", "Could not determine current user.");
                return View(model);
            }

            var ok = await _staffService.UpdateStaffAsync(model, currentUserId);

            if (!ok)
            {
                ModelState.AddModelError("", "Failed to update staff. Please try again.");
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            return RedirectToAction("StaffList");
        }

        // Student List
        public async Task<IActionResult> StudentList()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        // Edit Student
        [HttpGet]
        public async Task<IActionResult> EditStudent(long id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            var model = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Address = student.Address,
                Phone = student.Phone,
                Status = student.Status,
                DepartmentId= student.DepartmentId
            };

            ViewBag.Departments = _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();

            return View(model);
        }

        // Edit Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(StudentEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var currentUserId))
            {
                ModelState.AddModelError("", "Could not determine current user.");
                return View(model);
            }

            var ok = await _studentService.UpdateStudentAsync(model, currentUserId);

            if (!ok)
            {
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            return RedirectToAction("StudentList");
        }

        // Create Student
        [HttpGet]
        public IActionResult CreateStudent()
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
        public async Task<IActionResult> CreateStudent(StudentRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();

                return View(model);
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long? currentUserId = null;
            if (!string.IsNullOrEmpty(userIdString) && long.TryParse(userIdString, out var parsed))
                currentUserId = parsed;

            var ok = await _studentService.CreateStudentAsync(model, currentUserId);

            if (!ok)
            {
                ModelState.AddModelError("", "Failed to create Student. Please try again.");

                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();

                return View(model);
            }

            return RedirectToAction("DashBoard", "Admin");
        }




    }
}
