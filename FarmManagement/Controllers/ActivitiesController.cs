using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Controllers
{
    public class ActivitiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
