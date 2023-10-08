using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Booking
{
    public class BookingStatusUpdateDTO
    {
        [Required]
        public int BookingId { get; set; }
        [Required]
        public int JourneyId { get; set; }
        [Required]
        public string Status { get; set; }
        public string Comment { get; set; }
        public string? FileName { get; set; }
    }
}
