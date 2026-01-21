using HelpdeskService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Student_Complain_Management_System.Controllers
{
    [AllowAnonymous]
    public class PublicNoticeController : Controller
    {
        private readonly INoticeService _noticeService;

        public PublicNoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        //Notice for Student
        public async Task<IActionResult> Index()
        {
            var notices = await _noticeService.GetAllApprovedAsync();
            return View(notices);
        }
    }
}
