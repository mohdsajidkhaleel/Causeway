using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Customeraddress
    {
        public Customeraddress()
        {
            BookingCustomerAddresses = new HashSet<Booking>();
            BookingReceipientCustomerAddresses = new HashSet<Booking>();
        }

        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? Address { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? LocationId { get; set; }
        public string? Pincode { get; set; }
        public string? Mobile { get; set; }
        public string? Landmark { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual District? District { get; set; }
        public virtual Location? Location { get; set; }
        public virtual State? State { get; set; }
        public virtual ICollection<Booking> BookingCustomerAddresses { get; set; }
        public virtual ICollection<Booking> BookingReceipientCustomerAddresses { get; set; }
    }
}
