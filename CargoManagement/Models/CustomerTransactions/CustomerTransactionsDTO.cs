namespace CargoManagement.Models.CustomerTransactions
{
    public class CustomerTransactionsDTO
    {
        public decimal PreviousAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal NewAmount { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
