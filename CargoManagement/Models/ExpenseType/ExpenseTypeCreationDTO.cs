using System.ComponentModel.DataAnnotations;

namespace CargoManagement.Models.ExpenseType
{
    public class ExpenseTypeCreationDTO
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
