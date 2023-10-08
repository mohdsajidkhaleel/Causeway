namespace CargoManagement.Models.Hubs
{
    public class HubDetailsDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? StateId { get; set; }
        public string State { get; set; }
        public int? DistrictId { get; set; }
        public string District { get; set; }
        public int? LocationId { get; set; }
        public string Location { get; set; }
        public int? HubTypeId { get; set; }
        public string HubType { get; set; }
    }
}
