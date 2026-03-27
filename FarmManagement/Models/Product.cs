using System;
using System.Collections.Generic;

namespace FarmManagement.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
    }
}
