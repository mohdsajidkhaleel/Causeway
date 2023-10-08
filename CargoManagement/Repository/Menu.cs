using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Menu
    {
        public Menu()
        {
            Menuactions = new HashSet<Menuaction>();
        }

        public int Id { get; set; }
        public string MenuId { get; set; } = null!;
        public string? MainMenu { get; set; }
        public string? SubMenu { get; set; }
        public string? ParentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

        public virtual ICollection<Menuaction> Menuactions { get; set; }
    }
}
