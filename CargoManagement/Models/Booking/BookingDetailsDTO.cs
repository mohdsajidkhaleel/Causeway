using CargoManagement.Models.BookinItems;
using CargoManagement.Models.Customers;

namespace CargoManagement.Models.Booking
{
    public class BookingDetailsDTO
    {
        public int Id { get; set; }
        public string BookingId { get; set; } = null!;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
        public int ReceipientCustomerId { get; set; }
        public string ReceipientName { get; set; }
        public string ReceipientAddress { get; set; }
        public string? InvoiceNumber { get; set; }
        public string CurrentHubName { get; set; }
        public string OriginHubName { get; set; }
        public string Journey { get; set; }
        public string StatusId { get; set; }
        public string Status { get; set; }
        public decimal HandlingCharges { get; set; }
        public decimal FreightCharges { get; set; }
        public string ShipmentMode { get; set; }
        public decimal RoundOffAmnount { get; set; }
        public decimal NetAmnount { get; set; }
        public string PaymentModeName { get; set; }
        public string PaymentMode { get; set; }
        public bool? IsClosed { get; set; }
        public bool? IsCash { get; set; }
        public bool? IsPaymentCompleted { get; set; }
        public int TotalBoxCount { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Notes { get; set; }
        public List<BookingItemsListDTO> BookingItems { get; set; }
        public CustomerDetailsDTO Sender { get; set; }
        public CustomerDetailsDTO Recepient { get; set; }



    }
}
