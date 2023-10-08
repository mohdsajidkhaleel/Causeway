using CargoManagement.Models.User;

namespace CargoManagement.Services.Abstractions
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> Get();
        Task<IEnumerable<UserResponseDTO>> GetDrivers();
        Task<bool> Delete(int userId);
        Task<UserResponseDTO> Create(UserCreationDTO user);
        Task<UserResponseDTO> Update(UserUpdateDTO user);
        IQueryable<UserResponseDTO> GetDetailsAsQueryable(int? hubId);

    }
}
