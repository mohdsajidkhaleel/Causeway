using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Customerdiscount
    {
        public int Id { get; set; }
        public decimal DiscountAmount { get; set; }
        public int CustomerId { get; set; }
        public int DiscountGivenBy { get; set; }
        public string? DiscountBookingIds { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Customer Customer { get; set; } = null!;
    }
}
