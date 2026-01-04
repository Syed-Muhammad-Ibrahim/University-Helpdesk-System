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
        private readonly IConversationService _conversationService;

        public StaffComplainController(IConversationService conversationService, IComplainService complainService)
        {
            _conversationService = conversationService;
            _complainService = complainService;
        }

        // GET: /StaffComplain/DepartmentComplains
        public async Task<IActionResult> DepartmentComplains()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var complains = await _complainService.GetDepartmentComplainsForStaffAsync(userId);
            return View(complains);
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
                return NotFound();

            return View(complain);
        }

        public async Task<IActionResult> RepliedComplains()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var complains = await _conversationService.GetComplainsRepliedByUserAsync(userId);
            return View(complains);
        }

    }
}
