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
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ILogger<StaffController> _logger;
        private readonly IStaffService _staffService;
        private readonly INoticeService _noticeService;
        private readonly AppDbContext _context;

        public StaffController(ILogger<StaffController> logger,
                               IStaffService staffService,
                               INoticeService noticeService,
                               AppDbContext context)
        {
            _logger = logger;
            _staffService = staffService;
            _noticeService = noticeService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var staff = await _staffService.GetStaffByUserIdAsync(userId);
            if (staff == null)
                return NotFound();

            var model = new StaffUpdateViewModel
            {
                Id = staff.Id,
                Name = staff.Name,
                Address = staff.Address,
                Phone = staff.Phone,
                DepartmentId = staff.DepartmentId,
                Status = staff.Status
            };

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }

            return View("Profile", model);
        }

        // EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var staff = await _staffService.GetStaffByUserIdAsync(userId);
            if (staff == null)
                return NotFound();

            var model = new StaffUpdateViewModel
            {
                Id = staff.Id,
                Name = staff.Name,
                Address = staff.Address,
                Phone = staff.Phone,
                DepartmentId = staff.DepartmentId,
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

        // EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(StaffUpdateViewModel model)
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
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var ok = await _staffService.UpdateOwnProfileAsync(model, userId);
            if (!ok)
                return Unauthorized();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        // GET: /Staff/DepartmentNotices
        public async Task<IActionResult> DepartmentNotices()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var staff = await _staffService.GetStaffByUserIdAsync(userId);
            if (staff == null)
                return Unauthorized();

            var notices = await _noticeService.GetApprovedByDepartmentAsync(staff.DepartmentId);
            return View(notices);
        }
    }
}
