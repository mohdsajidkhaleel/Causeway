using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Customers
{
    public class CustomerUpdationDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
       
        public bool IsCreditAllowed { get; set; }
        public decimal? CreditLimit { get; set; }
    }
}
