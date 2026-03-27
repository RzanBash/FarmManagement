using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmManagement.Models;

public class ProductsController : Controller
{
    private readonly FarmBookingDBContext _context;

    public ProductsController(FarmBookingDBContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var products = _context.Products.ToList();
        return View(products);
    }
}

