namespace CargoManagement.Models.Journey
{
    public class JourneyFilterDTO
    {
        public DateTime? createdDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DateOfJourney { get; set; }
        public string? Status { get; set; }
    }
}
