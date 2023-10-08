using CargoManagement.Models.CustomerAddress;
using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Customers
{
    public class CustomerCreationDTO
    {
        public CustomerCreationDTO()
        { 
        
        }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Mobile { get; set; } = null!;
        
        public string? Email { get; set; }        
        public bool? IsCreditAllowed { get; set; }
        public decimal? CreditLimit { get; set; } = 0;
        public decimal? OutstandingCredit { get; set; } = 0;
       
        [Required]
        public List<CustomerAddressCreationDTO> Address { get; set; }
    }
}
