using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Bookingitem
    {
        public Bookingitem()
        {
            Bookingitemsdistributions = new HashSet<Bookingitemsdistribution>();
            Bookingpayments = new HashSet<Bookingpayment>();
            Journeyitems = new HashSet<Journeyitem>();
        }

        public int Id { get; set; }
        public int BookingId { get; set; }
        public int BoxTypeId { get; set; }
        public int Quantity { get; set; }
        public int PlannedQty { get; set; }
        public int InTransitQty { get; set; }
        public int ReceivedQty { get; set; }
        public int DeliveredQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Boxtype BoxType { get; set; } = null!;
        public virtual ICollection<Bookingitemsdistribution> Bookingitemsdistributions { get; set; }
        public virtual ICollection<Bookingpayment> Bookingpayments { get; set; }
        public virtual ICollection<Journeyitem> Journeyitems { get; set; }
    }
}
