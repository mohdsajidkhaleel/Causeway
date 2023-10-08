
using CargoManagement.Models.JourneyItem;

namespace CargoManagement.Models.Journey
{
    public class JourneyResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DriverId { get; set; }
        public int OriginHubId { get; set; }
        public int DestinationHubId { get; set; }
        public bool? IsLocal { get; set; }
        public string ContainerId { get; set; }
        public DateTime DateOfJourney { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public List<JourneyItemsResponseDTO> Items { get; set; }

    }
}
