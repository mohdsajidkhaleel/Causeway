namespace CargoManagement.Models.JourneyItem
{
    public class JourneyItemsCreationDTO
    {
        public int Id { get; set; }
        public int ItemDistributionId { get; set; }
        public int Quantity { get; set; }
        public int JourneyId { get; set; }
        public int BookingItemId { get; set; }
        public int? OriginHubId { get; set; }
        public int? DestinationHubId { get; set; }
        public string? Action { get; set; }
        public string? Notes { get; set; }
        public decimal? AdditionalCharge { get; set; }
    }
}
