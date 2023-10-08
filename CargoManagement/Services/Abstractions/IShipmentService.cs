using CargoManagement.Models.Booking;
using CargoManagement.Models.BookinItems;

namespace CargoManagement.Services.Abstractions
{
    public interface IShipmentService
    {
        Task<IEnumerable<BookingListDTO>> Get();
        Task<IEnumerable<BookingItemDistributionListDTO>> GetDeliverables();
        Task<IEnumerable<BookingListDTO>> GetPickups();

        Task<IEnumerable<BookingItemDistributionListDTO>> GetDeliverablesByHub(int hubId);
    }
}
