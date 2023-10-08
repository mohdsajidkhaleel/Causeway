using AutoMapper;
using CargoManagement.Models.Authentication;
using CargoManagement.Models.Shared;
using CargoManagement.Models.User;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CargoManagement.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CMSConfig _config;

        public AuthenticationService(cmspartialdeliveryContext context,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IOptions<CMSConfig> config)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _config = config.Value;

        }

        public async Task<bool> ChangePassword(string currentPassword, string newPassword)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId && x.Password == currentPassword);
            if (user != null)
            {
                user.Password = newPassword;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfile(UpdateProfileDTO user)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var userEntity = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userEntity != null)
            {
                userEntity.Email = user.Email;
                userEntity.Name = user.Name;
                userEntity.Mobile = user.Mobile;
                userEntity.AlternativeMobile = user.AlternativeMobile;
                _context.Update(userEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfilePic(string newProfilePic)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var entity = new User() { Id = userId, Image = newProfilePic };
            var attachedEntity = _context.Users.Attach(entity);
            attachedEntity.Property(x => x.Image).IsModified = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<AuthenticationResponseDTO> ValidateUser(AuthenticationRequestDTO customer)
        {
            var userInfo = await _context.Users
                .Include(x => x.UserRole)
                .FirstOrDefaultAsync(x => x.Username == customer.Username && x.Password == customer.Password);
            if (userInfo != null)
            {
                string jwt = GenerateJSONWebToken(userInfo);
                var aa = new AuthenticationResponseDTO()
                {
                    token = jwt,
                    Email = userInfo.Email,
                    Name = userInfo.Name,
                    Mobile = userInfo.Mobile,
                    Image = userInfo.Image,
                    IsAdminUser = userInfo.HubId == null ? true : false,
                    UserRoleId = userInfo.UserRoleId
                };
                return aa;
            }
            return null;
        }

        public async Task<MyProfileDTO> MyProfile()
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            MyProfileDTO mappedValues = _mapper.Map<MyProfileDTO>(user);
            if (mappedValues.Image != null)
            {
                mappedValues.ImageUrl = _config.FileDownloadUrl + "/" + _config.ProfileFolderAlias + "/" + mappedValues.Image;
            }
            return mappedValues;
        }
        private string GenerateJSONWebToken(User userInfo)
        {
            var claims = new[] {
        new Claim("UserId", userInfo.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
        new Claim("HubId", userInfo.HubId.ToString()),
        };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ANC987654321SECRET"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("ancbusiness.com",
             "ancbusiness.com",
              claims,
              expires: DateTime.Now.AddMinutes(1380), // changes to 23 hours session time expiration time.
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
