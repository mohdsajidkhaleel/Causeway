using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Authentication
{
    public class UpdateProfileDTO
    {
        [Required]
         public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }
        
        [Required]
        public string? Mobile { get; set; }
        
        [Required]
        public string? AlternativeMobile { get; set; }

    }
}
