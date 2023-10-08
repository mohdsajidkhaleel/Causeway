using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Usertype
    {
        public Usertype()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
