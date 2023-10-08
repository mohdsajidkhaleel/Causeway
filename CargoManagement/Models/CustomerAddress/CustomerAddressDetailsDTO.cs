namespace CargoManagement.Models.CustomerAddress
{
    public class CustomerAddressDetailsDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string Address { get; set; } = null!;
        public int StateId { get; set; }
        public string State { get; set; }
        public int DistrictId { get; set; }
        public string District { get; set; }
        public int LocationId { get; set; }
        public string? Location { get; set; }
        public string? Pincode { get; set; }
        public string? Mobile { get; set; }
        public string? Landmark { get; set; }
        public string? Description { get; set; }
    }
}
