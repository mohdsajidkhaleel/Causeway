namespace CargoManagement.Models.Expense
{
    public class GetExpenseResponseDTO
    {
        public int Id { get; set; }
        public int? JourneyId { get; set; }
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
