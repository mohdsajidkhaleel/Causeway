using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Authentication
{
    public class AuthenticationRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
