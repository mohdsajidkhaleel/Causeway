using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Menuaction
    {
        public Menuaction()
        {
            Userpermissions = new HashSet<Userpermission>();
        }

        public int Id { get; set; }
        public string ActionName { get; set; } = null!;
        public int MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? ActionType { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

        public virtual Menu Menu { get; set; } = null!;
        public virtual ICollection<Userpermission> Userpermissions { get; set; }
    }
}
