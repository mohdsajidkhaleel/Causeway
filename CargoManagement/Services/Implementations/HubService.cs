using AutoMapper;
using CargoManagement.Models.Hubs;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class HubService : IHubSerice
    {

        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HubService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<HubResponseDTO> Create(HubCreationDTO hub)
        {
            var newHub = _mapper.Map<Hub>(hub);
            await _context.Hubs.AddAsync(newHub);
            await _context.SaveChangesAsync();

            return _mapper.Map<HubResponseDTO>(newHub);
        }

        public async Task<bool> Delete(int hubId)
        {
            var itemToRemove = _context.Hubs.SingleOrDefault(x => x.Id == hubId);
            if (itemToRemove != null)
            {
                _context.Remove<Hub>(itemToRemove);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<HubResponseDTO>> Get()
        {
            var hubs = await _context.Hubs
               .ToListAsync();
            return _mapper.Map<IEnumerable<HubResponseDTO>>(hubs);
        }

        public IQueryable<HubDetailsDTO> GetDetailsAsQueryable()
        {
            var hubId = _httpContextAccessor.HttpContext.GetClaim("HubId");
            var hubs = _context.Hubs
                .Include(x => x.Location).ThenInclude(loc => loc.District).ThenInclude(district => district.State)
                .Include(x => x.HubType)
                .OrderBy(hub => hub.Name)
                .AsQueryable();
            return _mapper.ProjectTo<HubDetailsDTO>(hubs);
        }

        public async Task<HubResponseDTO> Update(HubUpdationDTO hub)
        {
            var hubToUpdate = _context.Hubs.SingleOrDefault(x => x.Id == hub.Id);
            if (hubToUpdate != null)
            {
                hubToUpdate.Name = hub.Name;
                hubToUpdate.Address = hub.Address;
                hubToUpdate.StateId = hub.StateId;
                hubToUpdate.DistrictId = hub.DistrictId;
                hubToUpdate.LocationId = hub.LocationId;

                _context.Update(hubToUpdate);
                await _context.SaveChangesAsync();
                return _mapper.Map<HubResponseDTO>(hubToUpdate);
            }
            else
            {
                return null;
            }
        }
    }
}
