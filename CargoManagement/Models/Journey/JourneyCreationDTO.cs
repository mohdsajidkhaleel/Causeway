using CargoManagement.Models.JourneyItem;
using CargoManagement.Validations;
using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Journey
{
    public class JourneyCreationDTO
    {
        public JourneyCreationDTO()
        { }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int DriverId { get; set; }
        [Required]
        public string ContainerId { get; set; }
        [Required]
        public bool IsLocal { get; set; }

        [RequiredIfMatchCondition(nameof(IsLocal), false, ErrorMessage = "Origin Hub is required")]
        public int? OriginHubId { get; set; }
        [RequiredIfMatchCondition(nameof(IsLocal), false, ErrorMessage = "Destination Hub is required")]
        public int? DestinationHubId { get; set; }
        [Required]
        public DateTime DateOfJourney { get; set; }
        public string? Notes { get; set; }

        public List<JourneyItemsCreationDTO> Items { get; set; }
    }
}
