namespace CargoManagement.Models.Customers
{
    public class CustomerBookingPaymentDTO
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int BookingId { get; set; }
        public int BookingItemId { get; set; }
        public DateTime? BookingDate { get; set; }
        public int? JourneyItemId { get; set; }
        public int? JourneyId { get; set; }
        public string? BookingCode { get; set; }
        public string? JourneyName { get; set; }
        public string? ConsignorName { get; set; }
        public string? ConsigneeName { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AmountToPay { get; set; }
        public int Quantity { get; set; }
        public int? DispatchedQuantity { get; set; }
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public string? BookingItemNote { get; set; }
    }

    public class CustomerMasterOutstandingPaymentDTO
    {
        public int Discount { get; set; }
        public int CustomerId { get; set; }
        public List<CustomerOutstandingPayment> PaymentList { get; set; }
    }
    public class CustomerOutstandingPayment
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int BookingId { get; set; }
        public int JourneyItemId { get; set; }
        public int JourneyId { get; set; }
    }

    public class PaymentHistoryDTO
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public int? JourneyId { get; set; }
        public string? BookingCode { get; set; }
        public string? JourneyName { get; set; }
        public string? ContainerName { get; set; }
        public string? ConsignorName { get; set; }
        public string? ConsigneeName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal? TotalAmountPaid { get; set; }        
        public decimal? Discount { get; set; }        
        public int? TotalPaidQuantity { get; set; }
        public DateTime? PaidDate { get; set; }
    }

    public class MonthlyPLReportDetailDTO
    {
        public MonthlyPLReportDetailDTO()
        {
            OfficeExpenses = new List<MonthlyOfficeExpenseReportDetailDTO>();
            JourneyAmountReceived = new List<MonthlyJourneyReportDetailDTO>();
            JobExpense = new List<MonthlyJobExpenseReportDetailDTO>();
        }
        public List<MonthlyOfficeExpenseReportDetailDTO> OfficeExpenses { get; set; }
        public List<MonthlyJourneyReportDetailDTO> JourneyAmountReceived { get; set; }
        public List<MonthlyJobExpenseReportDetailDTO> JobExpense { get; set; }
    }

    public class MonthlyOfficeExpenseReportDetailDTO
    {
        public string ExpenseName { get; set; }
        public decimal? ExpenseAmount { get; set; }
    }

    public class MonthlyJobExpenseReportDetailDTO
    {
        public string JobName { get; set; }
        public decimal? JobExpenseAmount { get; set; }
    }

    public class MonthlyJourneyReportDetailDTO
    {
        public string JobNo { get; set; }
        public decimal? ReceivedAmount { get; set; }
        //public string JobExpenseDesc { get; set; }
        //public decimal? JobExpenseAmount { get; set; }
    }

    public class DailyCollectionReportDetailDTO
    {
        public string BookingNumber { get; set; }
        public string JobNumber { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal? ReceivedAmount { get; set; }
    }

    public class JobCollectionReportDetailDTO
    {
        public JobCollectionReportDetailDTO()
        {
            JobExpenses = new List<JobExpenseReportDetailDTO>();
            JourneyCollection = new List<JobReportDetailDTO>();
        }
        public List<JobExpenseReportDetailDTO> JobExpenses { get; set; }
        public List<JobReportDetailDTO> JourneyCollection { get; set; }
    }

    public class JobReportDetailDTO
    {
        public string BookingNumber { get; set; }
        public DateTime? Date { get; set; }
        public decimal? ReceivedAmount { get; set; }
    }

    public class JobExpenseReportDetailDTO
    {
        public string ExpenseName { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public DateTime? Date { get; set; }
    }

}
