using CargoManagement.Models.Authentication;
using CargoManagement.Models.User;

namespace CargoManagement.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> ValidateUser(AuthenticationRequestDTO customer);
        Task<bool> ChangePassword(string currentPassword,string newPassword);
        Task<bool> UpdateProfilePic(string newProfilePic);
        Task<bool> UpdateProfile(UpdateProfileDTO user);
        Task<MyProfileDTO> MyProfile();
    }
}
