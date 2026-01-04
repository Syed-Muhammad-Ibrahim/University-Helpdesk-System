using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize(Roles = "Staff,Admin")]
    public class StaffComplainController : Controller
    {
        private readonly IComplainService _complainService;

        public StaffComplainController(IComplainService complainService)
        {
            _complainService = complainService;
        }

        // GET: /StaffComplain/DepartmentComplains
        public async Task<IActionResult> DepartmentComplains()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var complains = await _complainService.GetDepartmentComplainsForStaffAsync(userId);
            return View(complains); // Views/StaffComplain/DepartmentComplains.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var complain = await _complainService.GetByIdForStaffAsync(id, userId, isAdmin);

            if (complain == null)
                return NotFound(); // onnno department / nai

            return View(complain); // Views/StaffComplain/Details.cshtml
        }
    }
}
