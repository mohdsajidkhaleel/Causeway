
using CargoManagement.Models.JourneyItem;

namespace CargoManagement.Services.Abstractions
{
    public interface IJourneyItemsService
    {
        Task<IEnumerable<JourneyItemsResponseDTO>> Get(int journeyId);
        Task<JourneyItemsResponseDTO> GetJourneyDetailByID(int journeyItemId);
        Task<int> Delete(int Ids);
        Task<IEnumerable<JourneyItemsResponseDTO>> Create(List<JourneyItemsCreationDTO> journeyDetails);
        Task<int> DeliverItems(int[] Ids);
        Task<bool> PickupItems(int[] Ids);
        Task<IEnumerable<JourneyBookingItems>> GetBookingsForPickup(int journeyId, int locationID, bool locationIsHub);
        Task<IEnumerable<JourneyBookingItems>> GetBookingsForDelivery(int journeyId, int locationID, bool locationIsHub);
    }
}
