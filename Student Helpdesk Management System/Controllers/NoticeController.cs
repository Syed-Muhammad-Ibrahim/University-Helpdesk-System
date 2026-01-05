using HelpdeskModel.ViewModels;
using HelpdeskRepository.Data;
using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Staff,Admin")]
    public class NoticeController : Controller
    {
        private readonly INoticeService _noticeService;
        private readonly AppDbContext _context;

        public NoticeController(INoticeService noticeService, AppDbContext context)
        {
            _noticeService = noticeService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Admin");
            var notices = await _noticeService.GetAllApprovedAsync();

            return View(notices);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllNotice()
        {
            var notices = await _noticeService.GetAllAsync();
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
            await _noticeService.DeleteNoticeAsync(id, userId, isAdmin);

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
            return RedirectToAction(nameof(Pending));
        }

        // POST: Notice/Reject/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var adminId))
                return Unauthorized();

            await _noticeService.RejectAsync(id, adminId);
            return RedirectToAction(nameof(Pending));
        }
    }
}
