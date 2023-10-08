using AutoMapper;
using CargoManagement.Models.Booking;
using CargoManagement.Models.BookinItems;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class ShipmentService : IShipmentService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShipmentService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<BookingListDTO>> Get()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var list = await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.CustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.ReceipientCustomer)
                .Include(x => x.ReceipientCustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.CurrentHub)
                .Include(x => x.Status)
                .Include(x => x.Bookingitems)
                //.Where(x => x.CurrentHubId == hubId)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingListDTO>>(list);
        }
        public async Task<IEnumerable<BookingItemDistributionListDTO>> GetDeliverables()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var bookingItems = await _context.Bookingitemsdistributions
                .Where(d => d.HubId == hubId && d.Quantity > 0)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(customerAddress => customerAddress.CustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(customerAddress => customerAddress.ReceipientCustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(customer => customer.Customer)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(recCustomer => recCustomer.ReceipientCustomer)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingItemDistributionListDTO>>(bookingItems);
        }

        public async Task<IEnumerable<BookingItemDistributionListDTO>> GetDeliverablesByHub(int hubId)
        {

            var bookingItems = await _context.Bookingitemsdistributions
                .Where(d => d.HubId == hubId && d.Quantity > 0)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(customerAddress => customerAddress.CustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(customerAddress => customerAddress.ReceipientCustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(customer => customer.Customer)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.Booking).ThenInclude(recCustomer => recCustomer.ReceipientCustomer)
                .Include(bookingItems => bookingItems.BookingItem).ThenInclude(booking => booking.BoxType)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingItemDistributionListDTO>>(bookingItems);
        }

        public async Task<IEnumerable<BookingListDTO>> GetPickups()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var list = await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.CustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.ReceipientCustomer)
                .Include(x => x.ReceipientCustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.Status)
                .Where(x => x.OriginHubId == hubId && x.StatusId == BookingStatus.New && x.JourneyId == null)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingListDTO>>(list);
        }
    }
}
