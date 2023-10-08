namespace CargoManagement.Models.Dashboard
{
    public class DashboardResponseDTO
    {
        public int[] Ids { get; set; }
        public int Count { get; set; }
    }

    public class BookingDashboardResponseDTO
    {
        public int[] Ids { get; set; }
        public int Count { get; set; }
        public int TotalPackages { get; set; }
    }
}
