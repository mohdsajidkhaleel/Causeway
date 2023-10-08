using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Bookingstatus
    {
        public Bookingstatus()
        {
            Bookings = new HashSet<Booking>();
            Bookingtransactions = new HashSet<Bookingtransaction>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public sbyte? IsJourney { get; set; }
        public sbyte? IsHub { get; set; }
        public sbyte? IsCustomerStatus { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Bookingtransaction> Bookingtransactions { get; set; }
    }
}
