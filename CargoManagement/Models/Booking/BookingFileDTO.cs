namespace CargoManagement.Models.Booking
{
    public class BookingFileDTO
    {
        public string FileName { get; set; } = null!;
        public string? Remarks { get; set; }
        public string? Url { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
