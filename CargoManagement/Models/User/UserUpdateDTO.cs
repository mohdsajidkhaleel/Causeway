using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.User
{
    public class UserUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Mobile { get; set; }
        [Required]
        public int? UserRoleId { get; set; }
        public string? AlternativeMobile { get; set; }
    }
}
