using AutoMapper;
using CargoManagement.Models.ExpenseType;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;

namespace CargoManagement.Services.Implementations
{
    public class ExpenseTypeService : IExpenseTypeService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExpenseTypeService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ExpenseTypeCreationDTO> Create(ExpenseTypeCreationDTO expensetype)
        {
            var newExpensetype = _mapper.Map<Expensetype>(expensetype);
            await _context.Expensetypes.AddAsync(newExpensetype);
            await _context.SaveChangesAsync();

            return _mapper.Map<ExpenseTypeCreationDTO>(newExpensetype);
        }

        public async Task<bool> Delete(int expenseTypeId)
        {
            var itemToRemove = _context.Expensetypes.SingleOrDefault(x => x.Id == expenseTypeId);
            if (itemToRemove != null)
            {
                _context.Remove<Expensetype>(itemToRemove);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ExpenseTypeCreationDTO> Update(ExpenseTypeCreationDTO expensetype)
        {
            var ExpenseTypeToUpdate = _context.Expensetypes.SingleOrDefault(x => x.Id == expensetype.Id);
            if (ExpenseTypeToUpdate != null)
            {
                ExpenseTypeToUpdate.Name = expensetype.Name;
                ExpenseTypeToUpdate.Description = expensetype.Description;
                ExpenseTypeToUpdate.UpdatedBy = 1;

                _context.Update(ExpenseTypeToUpdate);
                await _context.SaveChangesAsync();
                return _mapper.Map<ExpenseTypeCreationDTO>(ExpenseTypeToUpdate);
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<PaymentStatusDTO>> GetPaymentStatus()
        {
            return new List<PaymentStatusDTO> {
                    new PaymentStatusDTO { PaymentStatusCode = "S", PaymentStatusName = "Self" },
                    new PaymentStatusDTO { PaymentStatusCode = "C", PaymentStatusName = "Credit" },
                    new PaymentStatusDTO { PaymentStatusCode = "T", PaymentStatusName = "ToPay" }
                };

        }
    }
}
