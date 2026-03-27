using System;
using System.Collections.Generic;

namespace FarmManagement.Models
{
    public partial class Cabin
    {
        public Cabin()
        {
            Bookings = new HashSet<Booking>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool? IsAvailable { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
