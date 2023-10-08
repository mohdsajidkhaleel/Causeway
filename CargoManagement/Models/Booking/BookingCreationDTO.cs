using CargoManagement.Models.BookinItems;
using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Booking
{
    public class BookingCreationDTO
    {
        public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }
        
        public int? CustomerAddressId { get; set; }
        [Required]
        public int ReceipientCustomerId { get; set; }
        
        public int? ReceipientCustomerAddressId { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal HandlingCharges { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal RoundOffAmnount { get; set; }
        [Required]
        public decimal NetAmnount { get; set; }
        [Required]
        public string? PaymentMode { get; set; }
        [Required]
        public string? ShipmentMode { get; set; }
        public string? Notes { get; set; }
        public bool isShipmentCollected { get; set; }
        public bool? IsCash { get; set; }

        [Required]
        public List<BookingItemsCreationDTO> BookingItems { get; set; }
    }
}
