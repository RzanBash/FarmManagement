using Microsoft.AspNetCore.Mvc;
namespace FarmManagement.Models
{
    public class AdminViewModel
    {
        public List<Product> Products { get; set; }
        public List<Cabin> Cabins { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}

