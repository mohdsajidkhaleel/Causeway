namespace CargoManagement.Models.Booking
{
    public class BookingFilterDTO
    {
        public string? BookingId { get; set; }
        public string? ConsignorName { get; set; }
        public string? ConsigneeName { get; set; }
        public string? Status { get; set; }
        public string? Mobile { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsClosed { get; set; }
    }
}
