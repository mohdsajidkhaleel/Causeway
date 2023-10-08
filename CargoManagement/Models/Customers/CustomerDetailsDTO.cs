using CargoManagement.Models.CustomerAddress;

namespace CargoManagement.Models.Customers
{
    public class CustomerDetailsDTO
    {
        public CustomerDetailsDTO()
        {
            Address = new List<CustomerAddressDetailsDTO>();
        }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsCreditAllowed { get; set; }
        public decimal? CreditLimit { get; set; } = 0;
        public decimal? OutstandingCredit { get; set; } = 0;
        public List<CustomerAddressDetailsDTO> Address { get; set; }
    }
}
