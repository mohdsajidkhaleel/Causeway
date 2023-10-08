using CargoManagement.Models.ExpenseType;

namespace CargoManagement.Services.Abstractions
{
    public interface IExpenseTypeService
    {
        Task<bool> Delete(int expenseTypeId);
        Task<ExpenseTypeCreationDTO> Create(ExpenseTypeCreationDTO hub);
        Task<ExpenseTypeCreationDTO> Update(ExpenseTypeCreationDTO hub);
        Task<IEnumerable<PaymentStatusDTO>> GetPaymentStatus();
    }
}
