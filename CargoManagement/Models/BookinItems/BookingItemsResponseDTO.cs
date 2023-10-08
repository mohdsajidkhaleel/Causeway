using CargoManagement.Models.Booking;
using CargoManagement.Models.Journey;

namespace CargoManagement.Models.BookinItems
{
    public class BookingItemsResponseDTO
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int BoxTypeId { get; set; }
        public int Quantity { get; set; }
        public int PlannedQty { get; set; }
        public int InTransitQty { get; set; }
        public int ReceivedQty { get; set; }
        public int DeliveredQty { get; set; }
        public int RemainingQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Description { get; set; }
    }
}
