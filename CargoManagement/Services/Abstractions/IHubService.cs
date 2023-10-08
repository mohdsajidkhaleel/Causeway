using CargoManagement.Models.Hubs;
using CargoManagement.Models.User;

namespace CargoManagement.Services.Abstractions
{
    public interface IHubSerice
    {
        Task<IEnumerable<HubResponseDTO>> Get();
        Task<bool> Delete(int hubId);
        Task<HubResponseDTO> Create(HubCreationDTO hub);
        Task<HubResponseDTO> Update(HubUpdationDTO hub);
        IQueryable<HubDetailsDTO> GetDetailsAsQueryable();
    }
}
