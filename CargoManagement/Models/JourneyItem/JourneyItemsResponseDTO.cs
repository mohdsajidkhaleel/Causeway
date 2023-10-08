using CargoManagement.Models.BookinItems;

namespace CargoManagement.Models.JourneyItem
{
    public class JourneyItemsResponseDTO
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int BookingId { get; set; }
        public int? BookingItemId { get; set; }
        public string BookingCode { get; set; }
        public DateTime? DateOfJourney { get; set; }
        public string JourneyName { get; set; }
        public int? BoxTypeId { get; set; }
        public int OriginHubId { get; set; }
        public int Quantity { get; set; }
        public string? PaymentMode { get; set; }
        public decimal UnitPrice { get; set; }
        public int? PaidBy { get; set; }
        public DateTime? PaymentDate { get; set; }        
        public decimal? PaidAmount { get; set; }
        public bool? IsPaymentSuccessfull { get; set; }
        public string? AmountToPay { get; set; }
        public int? DestinationHubId { get; set; }
        public string DestinationHubName { get; set; }
        public string? Action { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }

        public BookingItemsResponseDTO BookingItem { get; set; }

    }
}
