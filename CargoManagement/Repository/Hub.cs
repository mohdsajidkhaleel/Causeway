using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Hub
    {
        public Hub()
        {
            BookingCurrentHubs = new HashSet<Booking>();
            BookingNextHubs = new HashSet<Booking>();
            BookingOriginHubs = new HashSet<Booking>();
            Bookingitemsdistributions = new HashSet<Bookingitemsdistribution>();
            BookingtransactionCurrentHubs = new HashSet<Bookingtransaction>();
            BookingtransactionNextHubs = new HashSet<Bookingtransaction>();
            BookingtransactionOriginHubs = new HashSet<Bookingtransaction>();
            Customers = new HashSet<Customer>();
            JourneyCreatorHubs = new HashSet<Journey>();
            JourneyDestinationHubs = new HashSet<Journey>();
            JourneyOriginHubs = new HashSet<Journey>();
            JourneyitemDestinationHubs = new HashSet<Journeyitem>();
            JourneyitemOriginHubs = new HashSet<Journeyitem>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? LocationId { get; set; }
        public string? Pincode { get; set; }
        public int? HubTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual District? District { get; set; }
        public virtual Hubtype? HubType { get; set; }
        public virtual Location? Location { get; set; }
        public virtual State? State { get; set; }
        public virtual ICollection<Booking> BookingCurrentHubs { get; set; }
        public virtual ICollection<Booking> BookingNextHubs { get; set; }
        public virtual ICollection<Booking> BookingOriginHubs { get; set; }
        public virtual ICollection<Bookingitemsdistribution> Bookingitemsdistributions { get; set; }
        public virtual ICollection<Bookingtransaction> BookingtransactionCurrentHubs { get; set; }
        public virtual ICollection<Bookingtransaction> BookingtransactionNextHubs { get; set; }
        public virtual ICollection<Bookingtransaction> BookingtransactionOriginHubs { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Journey> JourneyCreatorHubs { get; set; }
        public virtual ICollection<Journey> JourneyDestinationHubs { get; set; }
        public virtual ICollection<Journey> JourneyOriginHubs { get; set; }
        public virtual ICollection<Journeyitem> JourneyitemDestinationHubs { get; set; }
        public virtual ICollection<Journeyitem> JourneyitemOriginHubs { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
