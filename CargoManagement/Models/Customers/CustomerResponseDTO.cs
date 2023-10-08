using CargoManagement.Models.CustomerAddress;

namespace CargoManagement.Models.Customers
{
    public class CustomerResponseDTO
    {
        public CustomerResponseDTO()
        {
            Address = new List<CustomerAdderssResponseDTO>();
        }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsCreditAllowed { get; set; }
        public decimal? CreditLimit { get; set; } = 0;
        public decimal? OutstandingCredit { get; set; } = 0;
        public int? HubId { get; set; }
        public List<CustomerAdderssResponseDTO> Address { get; set; }
    }

    public class DropdownCustomerResponseDTO
    {
        public DropdownCustomerResponseDTO()
        {
            Address = new List<CustomerAdderssResponseDTO>();
        }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<CustomerAdderssResponseDTO> Address { get; set; }
    }

    public class CustomerFilterDTO
    {
        public string? Name { get; set; }
        public string? Mobile { get; set; }
        public int? showCredit { get; set; }
    }
}
