using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Authentication
{
    public class ChangePasswordDTO
    {
        [Required]
        public string currentPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
}
