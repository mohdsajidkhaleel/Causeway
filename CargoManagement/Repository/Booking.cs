using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Booking
    {
        public Booking()
        {
            Bookingfiles = new HashSet<Bookingfile>();
            Bookingitems = new HashSet<Bookingitem>();
            Bookingpayments = new HashSet<Bookingpayment>();
            Bookingtransactions = new HashSet<Bookingtransaction>();
        }

        public int Id { get; set; }
        public string BookingId { get; set; } = null!;
        public int CustomerId { get; set; }
        public int? CustomerAddressId { get; set; }
        public int ReceipientCustomerId { get; set; }
        public int? ReceipientCustomerAddressId { get; set; }
        public int OriginHubId { get; set; }
        public int? NextHubId { get; set; }
        public int? CurrentHubId { get; set; }
        public string StatusId { get; set; } = null!;
        public string? ShipmentMode { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal HandlingCharges { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal RoundOffAmnount { get; set; }
        public decimal NetAmnount { get; set; }
        public decimal? TotalDiscountGiven { get; set; }
        public decimal? TotalAmountReceived { get; set; }
        public string? PaymentMode { get; set; }
        public int? PayLaterBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? PaidBy { get; set; }
        public string? PaymentRemarks { get; set; }
        public string? Notes { get; set; }
        public bool? IsCash { get; set; }
        public bool? IsClosed { get; set; }
        public string? ClosingRemarks { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? JourneyId { get; set; }
        public bool? IsEmailNotificationSent { get; set; }

        public virtual Hub? CurrentHub { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual Customeraddress? CustomerAddress { get; set; }
        public virtual Journey? Journey { get; set; }
        public virtual Hub? NextHub { get; set; }
        public virtual Hub OriginHub { get; set; } = null!;
        public virtual Customer ReceipientCustomer { get; set; } = null!;
        public virtual Customeraddress? ReceipientCustomerAddress { get; set; }
        public virtual Shipmentmode? ShipmentModeNavigation { get; set; }
        public virtual Bookingstatus Status { get; set; } = null!;
        public virtual ICollection<Bookingfile> Bookingfiles { get; set; }
        public virtual ICollection<Bookingitem> Bookingitems { get; set; }
        public virtual ICollection<Bookingpayment> Bookingpayments { get; set; }
        public virtual ICollection<Bookingtransaction> Bookingtransactions { get; set; }
    }
}
