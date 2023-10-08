using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Boxtype
    {
        public Boxtype()
        {
            Bookingitems = new HashSet<Bookingitem>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual ICollection<Bookingitem> Bookingitems { get; set; }
    }
}
