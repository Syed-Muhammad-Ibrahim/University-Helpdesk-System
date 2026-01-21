using HelpdeskModel.ViewModels;
using HelpdeskRepository.Data;
using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Staff,Admin")]
    public class NoticeController : Controller
    {
        private readonly INoticeService _noticeService;
        private readonly IStaffService _staffService;
        private readonly AppDbContext _context;

        public NoticeController(INoticeService noticeService, IStaffService staffService, AppDbContext context)
        {
            _noticeService = noticeService;
            _staffService = staffService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                var notices = await _noticeService.GetAllApprovedAsync();
                return View(notices);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var staff = await _staffService.GetStaffByUserIdAsync(userId);
            if (staff == null) return Unauthorized();

            var deptNotices = await _noticeService.GetApprovedByDepartmentAsync(staff.DepartmentId);
            return View(deptNotices);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllNotice(string? search, long? department, string? approved)
        {
            ViewBag.Departments = await _context.Departments
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDept = department;
            ViewBag.CurrentApproved = approved;

            var notices = await _noticeService.GetAllAsync();

            
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                notices = notices.Where(n =>
                    n.Description.ToLower().Contains(term) ||
                    (n.CreatedBy != null && n.CreatedBy.FullName.ToLower().Contains(term)) ||
                    (n.Department != null && n.Department.Name.ToLower().Contains(term))
                ).ToList();
            }

            // department filter
            if (department.HasValue)
                notices = notices.Where(n => n.DepartmentId == department.Value).ToList();

            // approved filter
            if (!string.IsNullOrWhiteSpace(approved))
            {
                if (approved == "yes") notices = notices.Where(n => n.isApproved).ToList();
                else if (approved == "no") notices = notices.Where(n => !n.isApproved).ToList();
            }

            notices = notices.OrderByDescending(n => n.CreatedAt).ToList();
            return View(notices);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var pending = await _noticeService.GetPendingNoticesAsync();
            return View(pending);
        }

        // GET: Notice/Create
        [HttpGet]
        public IActionResult Create()
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

        // POST: Notice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoticeViewModel model)
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
            {
                ModelState.AddModelError("", "Could not determine current user.");
                return View(model);
            }

            var isAdmin = User.IsInRole("Admin");
            var ok = await _noticeService.CreateNoticeAsync(model, userId, isAdmin);
            if (!ok)
            {
                ModelState.AddModelError("", "Failed to create notice.");
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            if (isAdmin)
                return RedirectToAction(nameof(AllNotice));
            else
                return RedirectToAction(nameof(Index));
        }

        // GET: Notice/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var notice = await _noticeService.GetByIdAsync(id);
            if (notice == null) return NotFound();

            var model = new NoticeViewModel
            {
                Id = notice.Id,
                Description = notice.Description,
                DepartmentId = notice.DepartmentId,
                AttachmentId = notice.AttachmentId
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

        // POST: Notice/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NoticeViewModel model)
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

            var isAdmin = User.IsInRole("Admin");
            var ok = await _noticeService.UpdateNoticeAsync(model, userId, isAdmin);
            if (!ok)
            {
                ModelState.AddModelError("", "Could not update notice.");
                ModelState.AddModelError("", "You are not allowed to edit an approved notice.");
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Notice/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var ok = await _noticeService.DeleteNoticeAsync(id, userId, isAdmin);

            if (!ok)
            {
                TempData["Error"] = "You are not allowed to delete this notice (it may be approved or not created by you).";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Notice/Approve/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var adminId))
                return Unauthorized();

            await _noticeService.ApproveAsync(id, adminId);
            return RedirectToAction(nameof(AllNotice));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var adminId))
                return Unauthorized();

            await _noticeService.RejectAsync(id, adminId);
            return RedirectToAction(nameof(AllNotice));
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> MyNotices()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var notices = await _noticeService.GetStaffNoticesAsync(userId);
            return View(notices);
        }
    }
}
