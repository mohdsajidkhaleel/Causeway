using CargoManagement.Models.Booking;
using CargoManagement.Models.JourneyItem;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;

namespace CargoManagement.Services.Abstractions
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDTO>> Get();
        Task<IEnumerable<DropDownDTO>> GetBookingCode();
        Task<int> Delete(int Id);
        Task<int> DeleteBookingItem(int Id);
        Task<BookingResponseDTO> Create(BookingCreationDTO hub);
        Task<BookingResponseDTO> Update(BookingCreationDTO hub);
        Task<IEnumerable<ViewBookingStatusResponseDTO>> ViewBookingStatus(int bookingId);
        //Task<IEnumerable<BookingListDTO>> GetList(BookingFilterDTO FilterList);
        IQueryable<BookingListDTO> GetList(BookingFilterDTO FilterList);
        Task<IEnumerable<BookingListDTO>> TodaysBookingExport(BookingFilterDTO FilterList);
        IQueryable<BookingListDTO> GetDetailsAsQueryable();
        Task<bool?> ReceivePayment(int Id, int customerId, string paymentRemarks);
        Task<bool> ReturnBooking(int bookingId);
        Task<BookingDetailsDTO> GetBookingDetails(int Id);
        Task<bool> UpdateJourneyOnBooking(List<JourneyItemsCreationDTO> bookingIds, int journeyId);
        Task<bool> UpdateBookingStatus(int bookingId, int journeyId, string status, string comment, string? fileName);
        Task<IEnumerable<BookingFileDTO>> GetBookingFiles(int bookingId);
        Task<IEnumerable<BookingTransactionsDTO>> GetBookingTransactions(int bookingId);
        Task<IEnumerable<BookingStatusCodesDTO>> GetBookingStatusCodes();

    }
}
