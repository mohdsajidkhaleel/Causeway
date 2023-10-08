using AutoMapper;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class DropdownService : IDropdownService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DropdownService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<BoxTypeDropDownDTO>> Boxtypes()
        {
            var list = await _context.Boxtypes
                .OrderBy(b => b.Name)
                  .Select(d => new BoxTypeDropDownDTO
                  {
                      Text = d.Name,
                      Value = d.Id,
                      UnitPrice = d.Price
                  }).ToListAsync();

            return list;
        }



        public async Task<IEnumerable<DropDownDTO>> States()
        {
            var list = await _context.States
                .OrderBy(b => b.Name)
                  .Select(d => new DropDownDTO
                  {
                      Text = d.Name,
                      Value = d.Id
                  }).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<DropDownDTO>> Districts(int stateId)
        {
            var list = await _context.Districts.
                Where(district => district.StateId == stateId)
                .OrderBy(b => b.Name)
                  .Select(d => new DropDownDTO
                  {
                      Text = d.Name,
                      Value = d.Id
                  }).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<DropDownDTO>> Locations(int districtId)
        {
            var list = await _context.Locations.
                Where(district => district.DistrictId == districtId)
                .OrderBy(b => b.Name)
                  .Select(d => new DropDownDTO
                  {
                      Text = d.Name + "-" + d.Pincode,
                      Value = d.Id
                  }).ToListAsync();

            return list;
        }
         public async Task<IEnumerable<DropDownDTO>> GetManifestJourneyDetails(DateTime DateofDispatch)
        {
            var list = await _context.Journeys
                .Where(x => x.DateOfJourney.Year == DateofDispatch.Year 
                && x.DateOfJourney.Month == DateofDispatch.Month 
                && x.DateOfJourney.Day == DateofDispatch.Day)
                .Select(d => new DropDownDTO
                {
                    Text = d.Name,
                    Value = d.Id
                }).ToListAsync();

            return list;
        }



        public async Task<IEnumerable<DropDownDTO>> UserTypes()
        {
            var list = await _context.Usertypes
                .OrderBy(b => b.Name)
                  .Select(d => new DropDownDTO
                  {
                      Text = d.Name,
                      Value = d.Id
                  }).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<DropDownDTO>> HubTypes()
        {
            var list = await _context.Hubtypes
                .OrderBy(b => b.Name)
                  .Select(d => new DropDownDTO
                  {
                      Text = d.Name,
                      Value = d.Id
                  }).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<object>> Status()
        {
            var query = from status in _context.Bookingstatuses.Where(status => Convert.ToBoolean(status.IsCustomerStatus) == true)
                        orderby status.Name ascending
                        select new
                        {
                            status.Name,
                            status.Id
                        };
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<DropDownDTO>> ExpenseTypes()
        {
            var list = await _context.Expensetypes
                .OrderBy(b => b.Name)
                  .Select(d => new DropDownDTO
                  {
                      Text = d.Name,
                      Value = d.Id
                  }).ToListAsync();

            return list;
        }
        public async Task<IEnumerable<BookingDropDownDTO>> GetBookingId()
        {
            return await _context.Bookings
                .Include(x => x.Bookingitems)
                .Where(x => x.StatusId != BookingStatus.DeliveredShipment &&
                       x.StatusId != BookingStatus.ReturnedToConsignor) // DS => Shipement Delivered
                .OrderByDescending(b => b.CreateDate)
                .Where(x => x.Bookingitems.Any(y => y.Quantity != (y.PlannedQty + y.InTransitQty + y.ReceivedQty + y.DeliveredQty)))
                        .Select(x => new BookingDropDownDTO
                        {
                            Text = x.BookingId,
                            BookingItemID = x.Bookingitems.Select(x=> x.Id).FirstOrDefault(),
                            Id = x.Bookingitems.Select(x => x.BookingId).FirstOrDefault()
                        })
                .ToListAsync();
        }
        public async Task<IEnumerable<DropDownDTO>> GetJourneyName()
        {
            return await _context.Journeys
                .Select(x => new DropDownDTO
                {
                    Text = x.Name,
                    Value = x.Id
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<DropDownDTO>> GetUserRoles()
        {
            return await _context.Userroles
                .Select(x => new DropDownDTO
                {
                    Text = x.UserRoleName,
                    Value = x.Id
                })
                .ToListAsync();
        }

    }
}
