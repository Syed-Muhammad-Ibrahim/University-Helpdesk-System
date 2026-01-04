using HelpdeskModel.ViewModels.UpdateViewModels;
using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ILogger<StaffController> _logger;
        private readonly IStaffService _staffService;

        public StaffController(ILogger<StaffController> logger,
                                 IStaffService staffService)
        {
            _logger = logger;
            _staffService = staffService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult EditProfile()
        {
            return View();
        }


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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(StaffUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var ok = await _staffService.UpdateOwnProfileAsync(model, userId);
            if (!ok)
                return Unauthorized();

            return RedirectToAction("Profile"); 
        }

    }
}
