using System;
using System.Collections.Generic;

namespace CargoManagement.Repository
{
    public partial class Customertransaction
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public decimal PreviousAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal NewAmount { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
