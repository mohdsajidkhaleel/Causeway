using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Journeyexpense
    {
        public int Id { get; set; }
        public int? JourneyId { get; set; }
        public int ExpenseTypeId { get; set; }
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Expensetype ExpenseType { get; set; } = null!;
        public virtual Journey? Journey { get; set; }
    }
}
