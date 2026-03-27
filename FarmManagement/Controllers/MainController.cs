using Microsoft.AspNetCore.Mvc;
using FarmManagement.Models; 

namespace FarmManagement.Controllers
{
    public class MainController : Controller
    {
        private readonly FarmBookingDBContext _context;

        public MainController(FarmBookingDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cabins()
        {
            var cabins = _context.Cabins.ToList();
            return View(cabins);
        }





    }
}
