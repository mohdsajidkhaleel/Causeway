using CargoManagement.Models.Shared;

namespace CargoManagement.Services.Abstractions
{
    public interface IDropdownService
    {
        Task<IEnumerable<BoxTypeDropDownDTO>> Boxtypes();
        Task<IEnumerable<DropDownDTO>> States();
        Task<IEnumerable<DropDownDTO>> Districts(int stateId);
        Task<IEnumerable<DropDownDTO>> GetManifestJourneyDetails(DateTime DateofDispatch);
        Task<IEnumerable<DropDownDTO>> Locations(int districtId);
        Task<IEnumerable<DropDownDTO>> UserTypes();
        Task<IEnumerable<DropDownDTO>> HubTypes();
        Task<IEnumerable<object>> Status();
        Task<IEnumerable<DropDownDTO>> ExpenseTypes();
        Task<IEnumerable<BookingDropDownDTO>> GetBookingId();
        Task<IEnumerable<DropDownDTO>> GetJourneyName();
        Task<IEnumerable<DropDownDTO>> GetUserRoles();
    }
}
