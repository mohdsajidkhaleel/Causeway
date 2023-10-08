using AutoMapper;
using CargoManagement.Models.BoxType;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class BoxTypesService : IBoxTypesService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoxTypesService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<BoxTypeResponseDTO>> GetListOnOrderByName()
        {
            var list = await _context.Boxtypes.OrderBy(b => b.Name).ToListAsync();
            return _mapper.Map<IEnumerable<BoxTypeResponseDTO>>(list);
        }
    }
}
