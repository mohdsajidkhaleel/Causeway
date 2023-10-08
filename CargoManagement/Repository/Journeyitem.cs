using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Journeyitem
    {
        public Journeyitem()
        {
            Bookingpayments = new HashSet<Bookingpayment>();
        }

        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int BookingItemId { get; set; }
        public int ItemDistributionId { get; set; }
        public int OriginHubId { get; set; }
        public int? DestinationHubId { get; set; }
        public int Quantity { get; set; }
        public string? PaymentMode { get; set; }
        public decimal UnitPrice { get; set; }
        public int? PaidBy { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? Status { get; set; }
        public string? Action { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Bookingitem BookingItem { get; set; } = null!;
        public virtual Hub? DestinationHub { get; set; }
        public virtual Bookingitemsdistribution ItemDistribution { get; set; } = null!;
        public virtual Journey Journey { get; set; } = null!;
        public virtual Hub OriginHub { get; set; } = null!;
        public virtual ICollection<Bookingpayment> Bookingpayments { get; set; }
    }
}
