using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Location
    {
        public Location()
        {
            Customeraddresses = new HashSet<Customeraddress>();
            Hubs = new HashSet<Hub>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? DistrictId { get; set; }
        public string? Pincode { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual District? District { get; set; }
        public virtual ICollection<Customeraddress> Customeraddresses { get; set; }
        public virtual ICollection<Hub> Hubs { get; set; }
    }
}
