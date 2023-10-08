namespace CargoManagement.Models.CustomerAddress
{
    public class CustomerAdderssResponseDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string Address { get; set; } = null!;
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int LocationId { get; set; }
        public string Pincode { get; set; } = null!;
        public string? Mobile { get; set; }
        public string? Landmark { get; set; }
        public string? Description { get; set; }
    }
}
