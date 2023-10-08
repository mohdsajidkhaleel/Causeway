using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Customer
    {
        public Customer()
        {
            BookingCustomers = new HashSet<Booking>();
            BookingReceipientCustomers = new HashSet<Booking>();
            Bookingpayments = new HashSet<Bookingpayment>();
            Customeraddresses = new HashSet<Customeraddress>();
            Customerdiscounts = new HashSet<Customerdiscount>();
            Customertransactions = new HashSet<Customertransaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public bool? IsCreditAllowed { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal? OutstandingCredit { get; set; }
        public int? HubId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Hub? Hub { get; set; }
        public virtual ICollection<Booking> BookingCustomers { get; set; }
        public virtual ICollection<Booking> BookingReceipientCustomers { get; set; }
        public virtual ICollection<Bookingpayment> Bookingpayments { get; set; }
        public virtual ICollection<Customeraddress> Customeraddresses { get; set; }
        public virtual ICollection<Customerdiscount> Customerdiscounts { get; set; }
        public virtual ICollection<Customertransaction> Customertransactions { get; set; }
    }
}
