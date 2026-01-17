using HelpdeskModel.ViewModels.UpdateViewModels;
using HelpdeskRepository.Data;
using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentService _studentService;
        private readonly AppDbContext _context;


        public StudentController(ILogger<StudentController> logger,
                                 IStudentService studentService,
                                 AppDbContext context)
        {
            _logger = logger;
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

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var student = await _studentService.GetStudentByUserIdAsync(userId); // ei method ta niche dekhai

            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Address = student.Address,
                Phone = student.Phone,
                DepartmentId = student.DepartmentId,
                Status = student.Status
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
        public async Task<IActionResult> EditProfile(StudentEditViewModel model)
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

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var currentUserId))
            {
                ModelState.AddModelError("", "Could not determine current user.");
                return View(model);
            }

            var ok = await _studentService.UpdateStudentAsync(model, currentUserId);
            if (!ok)
            {
                ModelState.AddModelError("", "Failed to update profile. Please try again.");
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }
            return RedirectToAction("DashBoard", "Student");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var student = await _studentService.GetStudentByUserIdAsync(userId);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentEditViewModel
            {
                Id = student.Id,
                StudentId = student.StudentId,
                Name = student.Name,
                Address = student.Address,
                Phone = student.Phone,
                DepartmentId = student.DepartmentId,
                Status = student.Status
            };
            ViewBag.DepartmentName = student.Department?.Name;

            return View(model);
        }

    }
}
