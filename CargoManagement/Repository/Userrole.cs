using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Userrole
    {
        public Userrole()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string UserRoleName { get; set; } = null!;
        public int? ParentId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
