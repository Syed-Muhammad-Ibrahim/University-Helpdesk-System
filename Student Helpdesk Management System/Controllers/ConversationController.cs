using HelpdeskModel.ViewModels.Conversation;
using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Student_Complain_Management_System.Controllers
{
    [Authorize]
    public class ConversationController : Controller
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet]
        public async Task<IActionResult> Thread(long complainId)
        {
            if (complainId <= 0)
                return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            var model = await _conversationService.GetThreadForUserAsync(complainId, userId, User);
            if (model == null) return NotFound();

            var isAdmin = User.IsInRole("Admin");
            var isStaff = User.IsInRole("Staff");
            var isStudent = User.IsInRole("Student");

            if (isAdmin)
            {
                ViewBag.BackController = "Admin";
                ViewBag.BackAction = "ComplainList";
            }
            else if (isStaff)
            {
                ViewBag.BackController = "StaffComplain";
                ViewBag.BackAction = "DepartmentComplains";
            }
            else if (isStudent)
            {
                ViewBag.BackController = "Complain";
                ViewBag.BackAction = "MyComplains";
            }
            else
            {
                ViewBag.BackController = "Home";
                ViewBag.BackAction = "Index";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostMessage(ConversationThreadViewModel model)
        {
            var msg = model.NewMessage;

            if (msg.ComplainId <= 0)
                return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                return Unauthorized();

            await _conversationService.AddMessageAsync(msg, userId);
            return RedirectToAction(nameof(Thread), new { complainId = msg.ComplainId });
        }
    }
}
