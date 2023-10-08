namespace CargoManagement.Models.JourneyItem
{
    public class JourneyBookingItems
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string BookingCode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddres { get; set; }
        public string? DistrictName { get; set; }
        public string? LocationName { get; set; }
        public string? Pincode { get; set; }
        public string? Mobile { get; set; }
        public string? AlternativeMobile { get; set; }
        public string? Landmark { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentType { get; set; }
        public int TotalBoxCount { get; set; }
    }
}
