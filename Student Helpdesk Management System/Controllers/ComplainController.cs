using Microsoft.AspNetCore.Mvc;

namespace Student_Complain_Management_System.Controllers
{
    public class ComplainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
