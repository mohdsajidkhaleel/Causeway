namespace CargoManagement.Models.Booking
{
    public class BookingListDTO
    {
         public int Id { get; set; }
        public string BookingId { get; set; } = null!;
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Mobile { get; set; }
        public string ReceipientName { get; set; }
        public string ReceipientAddress { get; set; }
        public string CurrentHubName { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? BookingItemDescription { get; set; }
        public string Status { get; set; } 
        public string StatusId { get; set; } 
        public decimal NetAmnount { get; set; }
        public string PaymentModeName { get; set; }
        public string Journey { get; set; }
        public string ShipmentMode { get; set; }
        public bool? IsPaymentCompleted { get; set; }
        public int TotalBoxCount { get; set; }
        public bool? IsClosed { get; set; }
        public bool? IsCash { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
