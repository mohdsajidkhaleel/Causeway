using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? AlternativeMobile { get; set; }
        public string? Image { get; set; }
        public int? UserTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? HubId { get; set; }
        public int? UserRoleId { get; set; }

        public virtual Hub? Hub { get; set; }
        public virtual Userrole? UserRole { get; set; }
        public virtual Usertype? UserType { get; set; }
    }
}
