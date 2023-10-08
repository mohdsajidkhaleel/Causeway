using CargoManagement.Models.Expense;
using CargoManagement.Models.Journey;

namespace CargoManagement.Services.Abstractions
{
    public interface IJourneyService
    {
        //Task<IEnumerable<JourneyResponseDTO>> Get(JourneyFilterDTO filter);
        IQueryable<JourneyResponseDTO> Get(JourneyFilterDTO filter);
        Task<int> Delete(int Id);
        Task<JourneyResponseDTO> Create(JourneyCreationDTO journey);
        Task<int> Update(JourneyCreationDTO journey);
        Task<IEnumerable<JourneyListDTO>> GetList();
        IQueryable<JourneyListDTO> GetDetailsAsQueryable();
        IQueryable<JourneyListDTO> GetScheduledDetailsAsQueryable();
        Task<JourneyResponseDTO> GetById(int Id);

        Task<bool> StartJourney(int Id);
        Task<bool> EndJourney(int Id);
        Task<bool> CancelJourney(int Id);

        Task<IEnumerable<JourneyListDTO>> GetMyJourneyList();
        Task<IEnumerable<JourneyLocationResponseDTO>> GetMyJourneyDeliveryLocations(int journeyId);
        Task<IEnumerable<JourneyLocationResponseDTO>> GetMyJourneyPickupLocations(int journeyId);
        Task<IEnumerable<JourneyExpenseCreationDTO>> CreateJourneyExpense(List<JourneyExpenseCreationDTO> expenses);
        Task<IEnumerable<JourneyExpenseCreationDTO>> UpdateExpenses(List<JourneyExpenseCreationDTO> expenses);
        //Task<IEnumerable<GetExpenseResponseDTO>> GetExpenseList(int? journeyId);
        IQueryable<GetExpenseResponseDTO> GetExpenseList(int? journeyId);
        Task<JourneyDeliveryNoteDTO> GetDeliveryNote(InputDeliverNoteDTO data);
        Task<IEnumerable<JourneyDeliveryNoteDTO>> GetAllDeliveryInvoiceNote(int JourneyId);
        Task<IEnumerable<JourneyDeliveryNoteDTO>> GetAllDeliveryNote(int JourneyId);
        Task<IEnumerable<JourneyStartExportDetailsDTO>> GetTodaysJourneyStartDetails(ManifestReportFilterDTO filterdata);
        Task<IEnumerable<GetJourneyforUnitPriceDTO>> GetJourneyDetailsUnitPrice(int journeyId);
        Task<bool> JourneyPayment(JourneyPaymentDTO paymentList);
        Task<bool> DeleteExpenses(int Id);
        Task<bool> UpdateUnitPrice(JourneyUpdateUnitPriceDTO UnitPriceList);
        void updateBookingStatusDetails(int[] bookingItemIds);
        Task<int> UpdatePayLaterBy(PayLaterDetailsDTO PayLaterDetails);
    }
}
