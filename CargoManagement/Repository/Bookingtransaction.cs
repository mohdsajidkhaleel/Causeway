using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Bookingtransaction
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int? OriginHubId { get; set; }
        public int? NextHubId { get; set; }
        public int? CurrentHubId { get; set; }
        public string StatusId { get; set; } = null!;
        public int? JourneyId { get; set; }
        public string? Remarks { get; set; }
        public string? FileName { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Hub? CurrentHub { get; set; }
        public virtual Journey? Journey { get; set; }
        public virtual Hub? NextHub { get; set; }
        public virtual Hub? OriginHub { get; set; }
        public virtual Bookingstatus Status { get; set; } = null!;
    }
}
