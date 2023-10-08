using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Shipmentmode
    {
        public Shipmentmode()
        {
            Bookings = new HashSet<Booking>();
        }

        public string ShipmentId { get; set; } = null!;
        public string ShipmentName { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
