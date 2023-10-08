using AutoMapper;
using CargoManagement.Models.User;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UserResponseDTO> Create(UserCreationDTO user)
        {
            var hubId = _httpContextAccessor.HttpContext.GetClaim("HubId");
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            var ValidateUser = await _context.Users.Where(x => x.Id == userId).SingleOrDefaultAsync();
            if (ValidateUser.HubId == null) // admin user
            {
                user.HubId = user.HubId; // admin can create other Hub users
            }
            else // other Users
            {
                user.HubId = Convert.ToInt32(hubId); // other users can create only their Hub users
            }
            var newUser = _mapper.Map<User>(user);
            newUser.UserId = Guid.NewGuid().ToString();
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(newUser);

        }

        public async Task<bool> Delete(int userId)
        {
            var itemToRemove = _context.Users.SingleOrDefault(x => x.Id == userId);
            if (itemToRemove != null)
            {
                _context.Remove<User>(itemToRemove);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<IEnumerable<UserResponseDTO>> Get()
        {
            var users = await _context.Users
                .Include(x => x.Hub)
                .Include(x => x.UserType)
                .Include(x => x.UserRole)
                .ToListAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        public IQueryable<UserResponseDTO> GetDetailsAsQueryable(int? hubId)
        {
            var userSessionHubId = _httpContextAccessor.HttpContext.GetClaim("HubId");
            int? applicableHubId = userSessionHubId == null ? (hubId == null ? null : hubId) : Convert.ToInt32(userSessionHubId);
            var users = _context.Users
                .Include(x => x.Hub)
                .Include(x => x.UserType)
                .Include(x => x.UserRole)
                .Where(user => user.HubId == (applicableHubId == null ? null : applicableHubId))
                .OrderBy(user => user.Name)
                .AsQueryable();
            return _mapper.ProjectTo<UserResponseDTO>(users);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetDrivers()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var users = await _context.Users
                .Include(x => x.Hub)
                .Include(x => x.UserType)
                .Where(user => user.UserTypeId == 2  ) //&& user.HubId == hubId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        public async Task<UserResponseDTO> Update(UserUpdateDTO user)
        {
            var userToUpdate = _context.Users.SingleOrDefault(x => x.Id == user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.AlternativeMobile = user.AlternativeMobile;
                userToUpdate.Name = user.Name;
                userToUpdate.Mobile = user.Mobile;
                userToUpdate.Email = user.Email;
                userToUpdate.UserRoleId = user.UserRoleId;
                _context.Update(userToUpdate);
                await _context.SaveChangesAsync();
                return _mapper.Map<UserResponseDTO>(userToUpdate);
            }
            else
            {
                return null;
            }
        }
    }
}
