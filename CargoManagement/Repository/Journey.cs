using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Journey
    {
        public Journey()
        {
            Bookingpayments = new HashSet<Bookingpayment>();
            Bookings = new HashSet<Booking>();
            Bookingtransactions = new HashSet<Bookingtransaction>();
            Journeyexpenses = new HashSet<Journeyexpense>();
            Journeyitems = new HashSet<Journeyitem>();
        }

        public int Id { get; set; }
        public int? DriverId { get; set; }
        public int OriginHubId { get; set; }
        public int DestinationHubId { get; set; }
        public string ContainerId { get; set; } = null!;
        public DateTime DateOfJourney { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string Name { get; set; } = null!;
        public int CreatorHubId { get; set; }
        public bool? IsLocal { get; set; }

        public virtual Hub CreatorHub { get; set; } = null!;
        public virtual Hub DestinationHub { get; set; } = null!;
        public virtual Hub OriginHub { get; set; } = null!;
        public virtual ICollection<Bookingpayment> Bookingpayments { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Bookingtransaction> Bookingtransactions { get; set; }
        public virtual ICollection<Journeyexpense> Journeyexpenses { get; set; }
        public virtual ICollection<Journeyitem> Journeyitems { get; set; }
    }
}
