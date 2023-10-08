using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.User
{
    public class UserCreationDTO
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string? Password { get; set; }
        
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Email { get; set; }
        
        [Required]
        public string? Mobile { get; set; }
        
        public string? AlternativeMobile { get; set; }
        
        [Required]
        public int UserTypeId { get; set; }
        [Required]
        public int? UserRoleId { get; set; }
        public int? HubId { get; set; }
    }
}
