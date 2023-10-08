using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.BookinItems
{
    public class BookingItemsCreationDTO
    {
        public int Id { get; set; }
        public int? BookingId { get; set; }
        [Required]
        public int BoxTypeId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }        
        public decimal TotalPrice { get; set; }
        public string? Description { get; set; }
    }
}
