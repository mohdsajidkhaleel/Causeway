using AutoMapper;
using CargoManagement.Models.Dashboard;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BookingDashboardResponseDTO> TodaysNewBookings()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            var totalBookings = await _context.Bookings
                .Include(x=> x.Bookingitems)
                .Where(x => x.CreateDate.Value.Year == DateTime.Today.Year && x.CreateDate.Value.Month == DateTime.Today.Month && x.CreateDate.Value.Day == DateTime.Today.Day)
                .ToListAsync();
            return new BookingDashboardResponseDTO()
            {
                Count = totalBookings.Count,
                Ids = totalBookings.Select(x => x.Id).ToArray(),
                TotalPackages = totalBookings.Sum(x=> x.Bookingitems.Sum(y=> y.Quantity))
            };
        }


        public async Task<DashboardResponseDTO> TodaysClosedBookings()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var totalClosed =  await _context.Bookings
              .Where(x => x.CreateDate.Value.Year == DateTime.Today.Year 
              && x.CreateDate.Value.Month == DateTime.Today.Month 
              && x.CreateDate.Value.Day == DateTime.Today.Day 
              && x.IsClosed == true)
              .ToListAsync();
            return new DashboardResponseDTO()
            {
                Count = totalClosed.Count,
                Ids = totalClosed.Select(x => x.Id).ToArray()
            };
        }

        public async Task<DashboardResponseDTO> TodaysScheduledJourneys()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var totalScheduled = await _context.Journeys
              .Where(x => x.DateOfJourney.Year == DateTime.Today.Year 
              && x.DateOfJourney.Month == DateTime.Today.Month 
              && x.DateOfJourney.Day == DateTime.Today.Day
              && x.Status != JourneyStatus.Cancelled)
              .ToListAsync();
            return new DashboardResponseDTO()
            {
                Count = totalScheduled.Count,
                Ids = totalScheduled.Select(x => x.Id).ToArray()
            };
        }
        public async Task<DashboardResponseDTO> OutstandingBookings()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var totalOutStanding = await _context.Bookings
              .Where(x => x.IsClosed != true)
              .ToListAsync();
            return new DashboardResponseDTO()
            {
                Count = totalOutStanding.Count,
                Ids = totalOutStanding.Select(x => x.Id).ToArray()
            };
        }

        public decimal? OutstandingCustomerCredit()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var sum = _context.Customers
                .Sum(cust => cust.OutstandingCredit);
            if (sum == null)
                return 0;
            else return sum;
        }
    }
}
