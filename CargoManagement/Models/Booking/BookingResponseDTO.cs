using CargoManagement.Models.BookinItems;

namespace CargoManagement.Models.Booking
{
    public class BookingResponseDTO
    {
        public int Id { get; set; }
        public string BookingId { get; set; } = null!;
        public int CustomerId { get; set; }
        public int CustomerAddressId { get; set; }
        public int ReceipientCustomerId { get; set; }
        public int ReceipientCustomerAddressId { get; set; }
        public string? InvoiceNumber { get; set; }
        public int OriginHubId { get; set; }
        public int? NextHubId { get; set; }
        public int? CurrentHubId { get; set; }
        public string StatusId { get; set; } = null!;
        public string ShipmentMode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal HandlingCharges { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal RoundOffAmnount { get; set; }
        public decimal NetAmnount { get; set; }
        public string? PaymentMode { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? PaidBy { get; set; }
        public string? PaymentRemarks { get; set; }
        public string? Notes { get; set; }
        public bool? IsClosed { get; set; }
        public string? ClosingRemarks { get; set; }
        public bool? IsCash { get; set; }
        public List<BookingItemsResponseDTO> BookingItems { get; set; }

    }
}
