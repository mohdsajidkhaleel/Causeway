using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Journey
{
    public class JourneyPaymentDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int BookingItemId { get; set; }
        [Required]
        public int JourneyId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public decimal PayAmount { get; set; }
    }
}
