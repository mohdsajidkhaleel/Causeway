using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Userpermissionaction
    {
        public Userpermissionaction()
        {
            Userpermissions = new HashSet<Userpermission>();
        }

        public int Id { get; set; }
        public string ActionName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Userpermission> Userpermissions { get; set; }
    }
}
