using HelpdeskModel.Models;
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
            ViewBag.DepartmentName = staff.Department?.Name;

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }

            return View("Profile", model);
        }

        // Update
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
 

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(StaffUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var currentUserId))
            {
                ModelState.AddModelError("", "Could not determine current user.");
                return View(model);
            }

            var ok = await _staffService.UpdateOwnProfileAsync(model, currentUserId);
            if (!ok)
            {
                ModelState.AddModelError("", "Failed to update profile. Please try again.");
                return View(model);
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        //DepartmentNotices
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
