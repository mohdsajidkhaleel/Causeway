using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Hubs
{
    public class HubCreationDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public int? StateId { get; set; }
        [Required]
        public int? DistrictId { get; set; }
        [Required]
        public int? LocationId { get; set; }
        [Required]
        public int? HubTypeId { get; set; }
    }
}
