namespace CargoManagement.Models.Booking
{
    public class ViewBookingStatusResponseDTO
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int BoxTypeId { get; set; }
        public string BoxTypeName { get; set; }
        public int Quantity { get; set; }
        public int PlannedQty { get; set; }
        public int InTransitQty { get; set; }
        public int ReceivedQty { get; set; }
        public int DeliveredQty { get; set; }
    }
}
