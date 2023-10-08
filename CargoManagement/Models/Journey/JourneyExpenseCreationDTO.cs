using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.Journey
{
    public class JourneyExpenseCreationDTO
    {

        public int Id { get; set; }
        public int? JourneyId { get; set; } // modified to nullable field to add the company other expense also, other than journey expense
        [Required]
        public int ExpenseTypeId { get; set; }
        public string? Notes { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
