using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Bookingpayment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int BookingItemId { get; set; }
        public int? JourneyId { get; set; }
        public int? JourneyItemId { get; set; }
        public string PaymentMode { get; set; } = null!;
        public int TotalQuantity { get; set; }
        public int? TotalDispatchedQuantity { get; set; }
        public decimal? TotalAmountToPay { get; set; }
        public decimal? TotalAmountPaid { get; set; }
        public decimal? Discount { get; set; }
        public decimal? AdditionalCharge { get; set; }
        public bool? IsPaymentCompleted { get; set; }
        public int? PayLaterBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? Paidby { get; set; }
        public DateTime? Paiddate { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Bookingitem BookingItem { get; set; } = null!;
        public virtual Journey? Journey { get; set; }
        public virtual Journeyitem? JourneyItem { get; set; }
        public virtual Customer? PaidbyNavigation { get; set; }
    }
}
