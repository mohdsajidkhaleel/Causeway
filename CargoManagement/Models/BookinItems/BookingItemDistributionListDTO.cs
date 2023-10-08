namespace CargoManagement.Models.BookinItems
{
    public class BookingItemDistributionListDTO
    {
        public int Id { get; set; }
        public int ItemDistributionId { get; set; }
        public string BookingId { get; set; } = null!;
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Mobile { get; set; }
        public string ReceipientName { get; set; }
        public string ReceipientAddress { get; set; }
        public string CurrentHubName { get; set; }
        public string BoxType { get; set; }
        public string BoxDescription { get; set; }
        public int TotalBoxCount { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
