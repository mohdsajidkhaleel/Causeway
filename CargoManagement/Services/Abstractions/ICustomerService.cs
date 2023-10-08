using CargoManagement.Models.CustomerAddress;
using CargoManagement.Models.Customers;
using CargoManagement.Models.CustomerTransactions;
using CargoManagement.Models.Shared;

namespace CargoManagement.Services.Abstractions
{
    public interface ICustomerService
    {
        //Task<IEnumerable<CustomerResponseDTO>> Get();
        IQueryable<CustomerResponseDTO> Get(CustomerFilterDTO filter);
        Task<IEnumerable<CustomerResponseDTO>> validateCustomerOutstandings(object customerList);
        Task<IEnumerable<CustomerResponseDTO>> GetTotalCustomerOutstandings();
        Task<IEnumerable<DropdownCustomerResponseDTO>> GetDropdownCustomer();
        Task<bool> Delete(int custId);
        Task<CustomerResponseDTO> Create(CustomerCreationDTO customer);
        Task<CustomerResponseDTO> Update(CustomerUpdationDTO customer);
        Task<CustomerDetailsDTO> GetCustomerByMobile(string mobileNumber);
        Task<CustomerDetailsDTO> GetCustomerById(int Id);
        Task<List<CustomerDetailsDTO>> GetCustomerLikeMobileOrName(string searchtext);
        IQueryable<CustomerDetailsDTO> GetDetailsAsQueryable();
        IQueryable<CustomerTransactionsDTO> GetCustomerTransactionAsQueryable(int customerId);

        Task<bool> ReceivePayment(int customerId, decimal amount);
        void UpdateCustomerCredit(int customerId, decimal amount, string bookingCode);

        Task<bool> UpdateCreditSetting(int customerId, bool isCredit, decimal creditLimit);


        //Customer address
        Task<CustomerAdderssResponseDTO> CreateAddress(CustomerAddressCreationDTO customerAddress);
        Task<CustomerAdderssResponseDTO> UpdateAddress(CustomerAddressCreationDTO customerAddress);
        Task<IEnumerable<CustomerBookingPaymentDTO>> YetToPayBookingDetails(int CustomerId); 
        Task<IEnumerable<PaymentHistoryDTO>> GetPaymentHistoryByCustomerId(int CustomerId); 
        Task<MonthlyPLReportDetailDTO> GetMonthlyReport(DateTime date);
        Task<IEnumerable<DailyCollectionReportDetailDTO>> GetDailyCollectionReport(DateTime date); 
        Task<JobCollectionReportDetailDTO> GetJobCollectionReport(int jobId); 
        Task<bool> DeleteAddress(int custAddressId);
        Task<bool> PayOutstandingAmount(CustomerMasterOutstandingPaymentDTO PayDetails);
    }
    
}
