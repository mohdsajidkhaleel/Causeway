using CargoManagement.Models.BoxType;

namespace CargoManagement.Services.Abstractions
{
    public interface IBoxTypesService
    {
        Task<IEnumerable<BoxTypeResponseDTO>> GetListOnOrderByName();

    }
}
