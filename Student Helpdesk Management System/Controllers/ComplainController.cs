using HelpdeskModel.BusinessRules;
using HelpdeskModel.Models;
using HelpdeskModel.ViewModels;      // ComplainViewModel ekhane thakbe
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
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            // Service diye complain create (business rule inside service)
            var ok = await _complainService.CreateComplainAsync(model, userId);
            if (!ok)
            {
                ModelState.AddModelError("", "Failed to create complain.");
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            return RedirectToAction(nameof(MyComplains));
        }

        // GET: Complain/MyComplains
        public async Task<IActionResult> MyComplains()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var complains = await _complainService.GetStudentComplainsAsync(userId);
            return View(complains); // MyComplains.cshtml
        }

        // GET: Complain/Edit/5
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

        // POST: Complain/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ComplainViewModel model)
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

            var ok = await _complainService.UpdateComplainAsync(model, userId);
            if (!ok)
            {
                ModelState.AddModelError("", "Could not update complain.");
                ViewBag.Departments = _context.Departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                return View(model);
            }

            return RedirectToAction(nameof(MyComplains));
        }

        // POST: Complain/Delete/5
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

            // sudhu oi student er complain dekhte parbe
            var complain = await _complainService.GetByIdForStudentAsync(id, userId);
            if (complain == null)
                return NotFound();

            return View(complain);   // View name: Details.cshtml, model: Complain
        }
    }
}
