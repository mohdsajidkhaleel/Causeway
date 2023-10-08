using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Bookingitemsdistribution
    {
        public Bookingitemsdistribution()
        {
            Journeyitems = new HashSet<Journeyitem>();
        }

        public int Id { get; set; }
        public int BookingItemId { get; set; }
        public int HubId { get; set; }
        public int Quantity { get; set; }
        public int InTransitQty { get; set; }
        public int ReceivedQty { get; set; }
        public int DeliveredQty { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Bookingitem BookingItem { get; set; } = null!;
        public virtual Hub Hub { get; set; } = null!;
        public virtual ICollection<Journeyitem> Journeyitems { get; set; }
    }
}
