using System;
using System.Collections.Generic;

namespace FarmManagement.Models
{
    public partial class Booking
    {
        public int Id { get; set; }
        public int CabinId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string? Status { get; set; }

        public virtual Cabin Cabin { get; set; } = null!;
    }
}
