namespace CargoManagement.Models.Journey
{
    public class JourneyListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DriverId { get; set; }
        //public string? DriverName { get; set; }
        public int OriginHubId { get; set; }
        public string? OriginHubName { get; set; }
        public int DestinationHubId { get; set; }
        public string? DestinationHubName { get; set; }
        public bool? IsLocal { get; set; }
        public string ContainerId { get; set; }
        public DateTime DateOfJourney { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public string? StatusName { get; set; }
        public int DeliveryCount { get; set; }
        public int PickupCount { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
