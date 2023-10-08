using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Userpermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MenuActionId { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

        public virtual Menuaction MenuAction { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
