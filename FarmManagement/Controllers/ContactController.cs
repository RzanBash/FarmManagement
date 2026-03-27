using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

