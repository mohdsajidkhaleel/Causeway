using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Bookingfile
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string FileName { get; set; } = null!;
        public string? Remarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }

        public virtual Booking Booking { get; set; } = null!;
    }
}
