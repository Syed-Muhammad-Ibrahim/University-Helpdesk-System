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
        private readonly IComplainService _complainService;

        public AdminController(
                                IStaffService staffService,
                                IStudentService studentService,
                                IComplainService complainService,
                                AppDbContext context)
        {
            _staffService = staffService;
            _studentService = studentService;
            _complainService = complainService;
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

        public async Task<IActionResult> ComplainList()
        {
            var complains = await _complainService.GetAllComplainsAsync();
            return View(complains);
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
            [HttpGet]
            public async Task<IActionResult> StaffList(string? search, long? department, string? status)
            {
                // dropdown departments
                ViewBag.Departments = await _context.Departments
                    .OrderBy(d => d.Name)
                    .ToListAsync();

                // default only active
                if (string.IsNullOrWhiteSpace(status))
                    status = "active";

                ViewBag.CurrentSearch = search;
                ViewBag.CurrentDept = department;
                ViewBag.CurrentStatus = status;

                var q = _context.Staffs
                    .Include(s => s.User)
                    .Include(s => s.Department)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.Trim().ToLower();
                    q = q.Where(s =>
                        s.Name.ToLower().Contains(term) ||
                        (s.User != null && s.User.Email.ToLower().Contains(term)));
                }

                if (department.HasValue)
                    q = q.Where(s => s.DepartmentId == department.Value);

                var st = status.Trim().ToLower();
                if (st == "active") q = q.Where(s => s.Status == ModelStatus.Active);
                else if (st == "inactive") q = q.Where(s => s.Status == ModelStatus.InActive);
                else if (st == "deleted") q = q.Where(s => s.Status == ModelStatus.Deleted);

                var staffs = await q.OrderBy(s => s.Name).ToListAsync();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(long id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var currentUserId))
                return Unauthorized();

            var ok = await _staffService.SoftDeleteStaffAsync(id, currentUserId);

            if (!ok) TempData["ErrorMessage"] = "Failed to delete staff.";
            else TempData["SuccessMessage"] = "Staff deleted successfully.";

            return RedirectToAction("StaffList");
        }

        // Student List
        [HttpGet]
        public async Task<IActionResult> StudentList(string? search, long? department, string? status)
        {
            ViewBag.Departments = await _context.Departments
                .OrderBy(d => d.Name)
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(status))
                status = "active";

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDept = department;
            ViewBag.CurrentStatus = status;

            var q = _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                q = q.Where(s =>
                    s.Name.ToLower().Contains(term) ||
                    (s.User != null && s.User.Email.ToLower().Contains(term)) ||
                    s.StudentId.ToString().Contains(term));
            }

            if (department.HasValue)
                q = q.Where(s => s.DepartmentId == department.Value);

            var st = status.Trim().ToLower();
            if (st == "active") q = q.Where(s => s.Status == ModelStatus.Active);
            else if (st == "inactive") q = q.Where(s => s.Status == ModelStatus.InActive);
            else if (st == "deleted") q = q.Where(s => s.Status == ModelStatus.Deleted);

            var students = await q.OrderBy(s => s.StudentId).ToListAsync();
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
                ModelState.AddModelError("StudentCode", "This Student Id is already taken.");
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

        // Soft Delete Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(long id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var currentUserId))
            {
                return Unauthorized();
            }

            var ok = await _studentService.SoftDeleteStudentAsync(id, currentUserId);
            if (!ok)
            {
                TempData["ErrorMessage"] = "Failed to delete student.";
            }
            else
            {
                TempData["SuccessMessage"] = "Student deleted successfully.";
            }

            return RedirectToAction("StudentList");
        }




    }
}
