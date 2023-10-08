﻿using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class District
    {
        public District()
        {
            Customeraddresses = new HashSet<Customeraddress>();
            Hubs = new HashSet<Hub>();
            Locations = new HashSet<Location>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? StateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual State? State { get; set; }
        public virtual ICollection<Customeraddress> Customeraddresses { get; set; }
        public virtual ICollection<Hub> Hubs { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
    }
}
