namespace CargoManagement.Models.Hubs
{
    public class HubResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? LocationId { get; set; }
        public int? HubTypeId { get; set; }
    }
}
