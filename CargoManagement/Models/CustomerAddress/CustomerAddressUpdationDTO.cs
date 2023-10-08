using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.CustomerAddress
{
    public class CustomerAddressUpdationDTO
    {
        [Required]
        public int Id { get; set; }
        
        public string? Address { get; set; } = null!;
       
        public int? StateId { get; set; }
       
        public int? DistrictId { get; set; }
        
        public int? LocationId { get; set; }
        
        public string? Pincode { get; set; } = null!;
        public string? Mobile { get; set; }
        public string? Landmark { get; set; }
        public string? Description { get; set; }
    }
}
