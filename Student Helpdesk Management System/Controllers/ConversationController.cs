using Microsoft.AspNetCore.Mvc;

namespace Student_Complain_Management_System.Controllers
{
    public class ConversationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
