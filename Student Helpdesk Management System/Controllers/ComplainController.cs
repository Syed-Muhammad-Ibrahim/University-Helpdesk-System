using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;
using HelpdeskService.Services;
using HelpdeskRepository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Student")]
    public class ComplainController : Controller
    {
        private readonly IComplainService _complainService;
        private readonly AppDbContext _context;

        public ComplainController(
            IComplainService complainService,
            AppDbContext context)
        {
            _complainService = complainService;
            _context = context;
        }

        // GET: Complain/Create
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

        // POST: Complain/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplainViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                    .ToList();
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                ModelState.AddModelError("", "Could not determine current user.");
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                    .ToList();
                return View(model);
            }

            if (model.File != null && model.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "complains");
                Directory.CreateDirectory(uploadsFolder);

                var storedName = $"{Guid.NewGuid()}{Path.GetExtension(model.File.FileName)}";
                var physicalPath = Path.Combine(uploadsFolder, storedName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var attachment = new Attachment
                {
                    FileNmae = model.File.FileName,
                    FileType = model.File.ContentType,
                    FilePath = "/uploads/complains/" + storedName,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    CreatedById = userId
                };

                _context.Attachments.Add(attachment);
                await _context.SaveChangesAsync();

                model.AttachmentId = attachment.Id;
            }

            var ok = await _complainService.CreateComplainAsync(model, userId);
            if (!ok)
            {
                ModelState.AddModelError("", "Failed to create complain.");

                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                    .ToList();
                return View(model);
            }

            return RedirectToAction(nameof(MyComplains));
        }

        //MyComplains
        public async Task<IActionResult> MyComplains()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var complains = await _complainService.GetStudentComplainsAsync(userId);
            return View(complains);
        }

        //Update
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var complain = await _complainService.GetByIdForStudentAsync(id, userId);
            if (complain == null)
                return NotFound();

            var model = new ComplainViewModel
            {
                Id = complain.Id,
                Description = complain.Description,
                DepartmentId = complain.DepartmentId,
                AttachmentId = complain.AttachmentId
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
        public async Task<IActionResult> Edit(ComplainViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                    .ToList();
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            if (model.File != null && model.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "complains");
                Directory.CreateDirectory(uploadsFolder);

                var storedName = $"{Guid.NewGuid()}{Path.GetExtension(model.File.FileName)}";
                var physicalPath = Path.Combine(uploadsFolder, storedName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var attachment = new Attachment
                {
                    FileNmae = model.File.FileName,
                    FileType = model.File.ContentType,
                    FilePath = "/uploads/complains/" + storedName,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    CreatedById = userId
                };

                _context.Attachments.Add(attachment);
                await _context.SaveChangesAsync();

                model.AttachmentId = attachment.Id;
            }

            var ok = await _complainService.UpdateComplainAsync(model, userId);
            if (!ok)
            {
                ModelState.AddModelError("", "Could not update complain.");

                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                    .ToList();
                return View(model);
            }

            return RedirectToAction(nameof(MyComplains));
        }

        //Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            await _complainService.DeleteComplainAsync(id, userId);
            return RedirectToAction(nameof(MyComplains));
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var complain = await _complainService.GetByIdForStudentAsync(id, userId);
            if (complain == null)
                return NotFound();

            return View(complain);   
        }
    }
}
