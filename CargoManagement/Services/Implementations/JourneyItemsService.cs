using AutoMapper;
using CargoManagement.Models.JourneyItem;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class JourneyItemsService : IJourneyItemsService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJourneyService _journeyService;
        public JourneyItemsService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJourneyService journeyService)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _journeyService = journeyService;
        }
        public async Task<IEnumerable<JourneyItemsResponseDTO>> Create(List<JourneyItemsCreationDTO> journeyDetails)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var BookingItemIds = journeyDetails.Select(x => x.BookingItemId).ToArray();
            //Getting basic details of Journey
            var journey = await _context.Journeys
                .FirstOrDefaultAsync(x => x.Id == journeyDetails[0].JourneyId);

            var bookingDetails = await _context.Bookingitems
                .Include(x=> x.Booking)
                .Where(x => BookingItemIds.Any(y => y == x.Id))
                .ToListAsync();

            //Preparing object for insert
            List<Journeyitem> newEntries = _mapper.Map<List<Journeyitem>>(journeyDetails);
            newEntries = newEntries.Select(c =>
            {
                c.DestinationHubId = journey.DestinationHubId;
                c.OriginHubId = journey.OriginHubId;
                c.PaymentMode = bookingDetails.SingleOrDefault(x => x.Id == c.BookingItemId)?.Booking?.PaymentMode;
                //c.OriginHubId = hubId;
                c.CreatedBy = userId;
                c.Action = "D";
                c.Status = JourneyShipmentStatus.Scheduled;
                var distribution = _context.Bookingitemsdistributions
                 .Include(x => x.BookingItem)
                 .Where(y => y.BookingItemId == c.BookingItemId)
                 .SingleOrDefault();
                c.ItemDistributionId = distribution.Id;
                distribution.BookingItem.PlannedQty = distribution.BookingItem.PlannedQty + c.Quantity;
                return c;
            }).ToList();
            //Adding and saving
            await _context.Journeyitems.AddRangeAsync(newEntries);
            await _context.SaveChangesAsync();

            //Return response view
            return _mapper.Map<List<JourneyItemsResponseDTO>>(newEntries);
        }

        public async Task<int> Delete(int Ids)
        {
            var itemsToRemove = await _context.Journeyitems
                .Include(x => x.Journey)
                .Include(x => x.BookingItem)
                .Where(x => x.Id == Ids).SingleOrDefaultAsync();

            if (itemsToRemove != null)
            {
                if (itemsToRemove.Journey.Status.Equals(JourneyShipmentStatus.Scheduled))
                {
                    itemsToRemove.BookingItem.PlannedQty = itemsToRemove.BookingItem.PlannedQty - itemsToRemove.Quantity;
                    _context.Remove<Journeyitem>(itemsToRemove);
                    await _context.SaveChangesAsync();
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return 0;
            }
        }

        public async Task<IEnumerable<JourneyItemsResponseDTO>> Get(int journeyId)
        {
            var hubs = await _context.Journeyitems
                .Include(x=> x.BookingItem).ThenInclude(x=> x.Booking)
                .Where(x => x.JourneyId == journeyId)
               .ToListAsync();
            return _mapper.Map<IEnumerable<JourneyItemsResponseDTO>>(hubs);
        }
        public async Task<JourneyItemsResponseDTO> GetJourneyDetailByID(int journeyItemId)
        {
            var data = await _context.Journeyitems
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking)
                .Where(x => x.Id == journeyItemId).SingleOrDefaultAsync();
            return _mapper.Map<JourneyItemsResponseDTO>(data);
        }


        public async Task<int> DeliverItems(int[] Ids)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var journeyId = 0;
            var itemsToDeliver = await _context.Journeyitems
                .Include(x => x.Journey)
                .Include(x => x.BookingItem)
                .ThenInclude(bookingItem => bookingItem.Booking)
                .Include(x => x.BookingItem).ThenInclude(x => x.Bookingitemsdistributions)
                .Where(x => Ids.Any(i => i == x.Id))
                .ToListAsync();
            var _getBookingItemIds = itemsToDeliver.Select(x => x.BookingItemId).Distinct().ToArray();

            if (itemsToDeliver.Where(x => x.Status.Equals(JourneyShipmentStatus.Received)).Count() > 0)
            {
                //Checking whether there are some outstanding to pay before proceeding with delivery
                if (itemsToDeliver.Where(item => item.BookingItem.Booking.PaymentMode != PaymentStatus.ToPay).Count() == 0)
                {
                    itemsToDeliver = itemsToDeliver.Select(c =>
                    {
                        //Items update
                        c.Status = JourneyShipmentStatus.Delivered;
                        c.UpdatedBy = userId;
                        journeyId = c.JourneyId;

                        //Curresponding bookings update
                        c.BookingItem.Booking.StatusId = c.DestinationHubId == null ? BookingStatus.DeliveredShipment : c.BookingItem.Booking.StatusId;
                        c.BookingItem.Booking.CurrentHubId = c.DestinationHubId == null ? null : c.DestinationHubId;
                        c.BookingItem.Booking.IsClosed = c.DestinationHubId == null ? true : c.BookingItem.Booking.IsClosed;
                        c.BookingItem.Booking.JourneyId = null;
                        c.BookingItem.Booking.NextHubId = null;
                        c.BookingItem.UpdatedBy = userId;
                        c.BookingItem.ReceivedQty = c.BookingItem.ReceivedQty - c.Quantity;
                        c.BookingItem.DeliveredQty = c.BookingItem.DeliveredQty + c.Quantity;

                        //Curresponding bookingDistributionItem update
                        foreach (var item in c.BookingItem.Bookingitemsdistributions)
                        {
                            if (item.BookingItemId == c.BookingItem.Id)
                            {
                                item.ReceivedQty = item.ReceivedQty - c.Quantity;
                                item.DeliveredQty = item.DeliveredQty + c.Quantity;
                                item.UpdatedBy = userId;
                            }
                        }

                        //Booking transaction history
                        c.BookingItem.Booking.Bookingtransactions.Add(new Bookingtransaction() { BookingId = c.BookingItem.Booking.Id, StatusId = (c.DestinationHubId == null ? BookingStatus.DeliveredShipment.ToString() : BookingStatus.DeliverToHub), CurrentHubId = c.DestinationHubId, CreatedBy = userId });
                        return c;
                    }).ToList();
                }
                else if (itemsToDeliver.Where(item => item.PaymentMode == PaymentStatus.ToPay).Count() == 0)
                {
                    itemsToDeliver = itemsToDeliver.Select(c =>
                    {
                        //Items update
                        c.Status = JourneyShipmentStatus.Delivered;
                        c.UpdatedBy = userId;
                        journeyId = c.JourneyId;

                        //Curresponding bookings update
                        c.BookingItem.Booking.StatusId = c.DestinationHubId == null ? BookingStatus.DeliveredShipment : c.BookingItem.Booking.StatusId;
                        c.BookingItem.Booking.CurrentHubId = c.DestinationHubId == null ? null : c.DestinationHubId;
                        c.BookingItem.Booking.IsClosed = c.DestinationHubId == null ? true : c.BookingItem.Booking.IsClosed;
                        c.BookingItem.Booking.JourneyId = null;
                        c.BookingItem.Booking.NextHubId = null;
                        c.BookingItem.UpdatedBy = userId;
                        // Journey Status Update

                        c.Journey.Status = JourneyStatus.Delivered;

                        //update in Quantity transfered
                        c.BookingItem.ReceivedQty = c.BookingItem.ReceivedQty - c.Quantity;
                        c.BookingItem.DeliveredQty = c.BookingItem.DeliveredQty + c.Quantity;

                        //Curresponding bookingDistributionItem update
                        foreach (var item in c.BookingItem.Bookingitemsdistributions)
                        {
                            if (item.BookingItemId == c.BookingItem.Id)
                            {
                                item.ReceivedQty = item.ReceivedQty - c.Quantity;
                                item.DeliveredQty = item.DeliveredQty + c.Quantity;
                                item.UpdatedBy = userId;
                            }
                        }
                        //Booking transaction history
                        c.BookingItem.Booking.Bookingtransactions.Add(new Bookingtransaction() { BookingId = c.BookingItem.Booking.Id, StatusId = (c.DestinationHubId == null ? BookingStatus.DeliveredShipment.ToString() : BookingStatus.DeliverToHub), CurrentHubId = c.DestinationHubId, CreatedBy = userId });
                        return c;
                    }).ToList();
                }
                else
                {
                    return 2;//Unable to procced delivery as there are outstanding to payments collected before delivery
                }
                _context.UpdateRange(itemsToDeliver);
                await _context.SaveChangesAsync();

                if(_getBookingItemIds.Count() > 0)
                {
                    _journeyService.updateBookingStatusDetails(_getBookingItemIds);
                }
                return 1; //returning if jouney deliver successfull
            }
            return 3; // returning if journey is not ended.

        }

        public async Task<bool> PickupItems(int[] Ids)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));

            //get details of  items to deliver
            var itemsToPickup = _context.Journeyitems
                .Include(x => x.BookingItem.Booking)
                .Where(x => Ids.Any(i => i == x.Id)).ToList();

            if (itemsToPickup.Count > 0)
            {
                itemsToPickup = itemsToPickup.Select(c =>
                {


                    //Curresponding bookings update
                    c.BookingItem.Booking.CurrentHubId = null;
                    c.BookingItem.Booking.StatusId = c.DestinationHubId == null ? BookingStatus.CollectedForShipment : BookingStatus.InTransit;//Customer collection will update booking status to collected else in transit
                    c.BookingItem.Booking.NextHubId = c.DestinationHubId == null ? null : c.DestinationHubId;//While collecting from hub ,next hub will be set from destination hub
                    c.UpdatedBy = userId;

                    //Items update ** It should be done only after the booking update as sequence matters
                    c.Status = JourneyShipmentStatus.Pickedup;
                    c.Action = JourneyShipmentAction.Delivery;
                    c.DestinationHubId = c.DestinationHubId == null ? hubId : c.DestinationHubId;
                    c.UpdatedBy = userId;

                    //Booking transaction history
                    c.BookingItem.Booking.Bookingtransactions.Add(new Bookingtransaction() { BookingId = c.BookingItem.Booking.Id, StatusId = (c.DestinationHubId == null ? BookingStatus.CollectedForShipment.ToString() : BookingStatus.PickedUp), CreatedBy = userId });

                    //return updated object
                    return c;
                }).ToList();

                //Updating changes to repo
                _context.UpdateRange(itemsToPickup);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<JourneyBookingItems>> GetBookingsForPickup(int journeyId, int locationID, bool locationIsHub)
        {

            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            return await _context.Journeyitems
                .Include(x => x.OriginHub).ThenInclude(hub => hub.Location).ThenInclude(hub => hub.District)
                .Include(x => x.BookingItem.Booking).ThenInclude(hub => hub.Customer)
                .Include(x => x.BookingItem.Booking).ThenInclude(hub => hub.Bookingitems)
                .Include(x => x.BookingItem.Booking).ThenInclude(hub => hub.ReceipientCustomer)
                .Include(y => y.BookingItem.Booking).ThenInclude(address => address.CustomerAddress).ThenInclude(loc => loc.Location).ThenInclude(loc => loc.District)
                .Include(y => y.BookingItem.Booking).ThenInclude(address => address.ReceipientCustomerAddress).ThenInclude(loc => loc.Location).ThenInclude(loc => loc.District)
                .Where(
                x => x.JourneyId == journeyId
                && x.BookingItem.Booking.CustomerAddress.LocationId == (locationIsHub ? x.BookingItem.Booking.CustomerAddress.LocationId : locationID)
                //&& x.OriginHubId == (locationIsHub ? locationID : x.OriginHubId)
                && x.Action == JourneyShipmentAction.Pickup)
                .OrderBy(x => x.BookingItem.Booking.ReceipientCustomer.Name)
                .Select(x => new JourneyBookingItems
                {
                    Id = x.Id,
                    BookingId = x.BookingItem.Booking.Id,
                    BookingCode = x.BookingItem.Booking.BookingId,
                    CustomerName = locationIsHub ? x.BookingItem.Booking.ReceipientCustomer.Name : x.BookingItem.Booking.Customer.Name,
                    CustomerAddres = locationIsHub ? x.BookingItem.Booking.ReceipientCustomerAddress.Address : x.BookingItem.Booking.CustomerAddress.Address,
                    DistrictName = locationIsHub ? x.BookingItem.Booking.ReceipientCustomerAddress.District.Name : x.BookingItem.Booking.CustomerAddress.District.Name,
                    LocationName = locationIsHub ? x.BookingItem.Booking.ReceipientCustomerAddress.Location.Name : x.BookingItem.Booking.CustomerAddress.Location.Name,
                    Pincode = locationIsHub ? x.BookingItem.Booking.ReceipientCustomerAddress.Location.Pincode : x.BookingItem.Booking.CustomerAddress.Location.Pincode,
                    Landmark = locationIsHub ? x.BookingItem.Booking.ReceipientCustomerAddress.Landmark : x.BookingItem.Booking.CustomerAddress.Landmark,
                    IsPaid = x.BookingItem.Booking.PaidDate == null ? false : true,
                    Mobile = locationIsHub ? x.BookingItem.Booking.ReceipientCustomer.Mobile : x.BookingItem.Booking.Customer.Mobile,
                    AlternativeMobile = locationIsHub ? x.BookingItem.Booking.ReceipientCustomerAddress.Mobile : x.BookingItem.Booking.CustomerAddress.Mobile,
                    TotalBoxCount = x.BookingItem.Booking.Bookingitems.Sum(x => x.Quantity)
                    //PaymentType = mapPaymentType(x.Booking.PaymentMode)
                })
                .ToListAsync();

        }

        public async Task<IEnumerable<JourneyBookingItems>> GetBookingsForDelivery(int journeyId, int locationID, bool locationIsHub)
        {

            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            return await _context.Journeyitems
                 .Include(x => x.DestinationHub).ThenInclude(hub => hub.Location).ThenInclude(hub => hub.District)
                 .Include(x => x.BookingItem.Booking).ThenInclude(hub => hub.ReceipientCustomer)
                 .Include(y => y.BookingItem.Booking).ThenInclude(address => address.ReceipientCustomerAddress).ThenInclude(loc => loc.Location).ThenInclude(loc => loc.District)
                 .Where(
                 x => x.JourneyId == journeyId
                 && x.BookingItem.Booking.ReceipientCustomerAddress.LocationId == (locationIsHub ? x.BookingItem.Booking.ReceipientCustomerAddress.LocationId : locationID)
                 //&& x.DestinationHubId == (locationIsHub ? locationID : x.DestinationHubId)
                 && x.Action == JourneyShipmentAction.Delivery && x.Status != JourneyShipmentStatus.Delivered)
                 .OrderBy(x => x.BookingItem.Booking.ReceipientCustomer.Name)
                 .Select(x => new JourneyBookingItems
                 {
                     Id = x.Id,
                     BookingId = x.BookingItem.Booking.Id,
                     BookingCode = x.BookingItem.Booking.BookingId,
                     CustomerName = x.BookingItem.Booking.ReceipientCustomer.Name,
                     CustomerAddres = x.BookingItem.Booking.ReceipientCustomerAddress.Address,
                     DistrictName = x.BookingItem.Booking.ReceipientCustomerAddress.District.Name,
                     LocationName = x.BookingItem.Booking.ReceipientCustomerAddress.Location.Name,
                     Pincode = x.BookingItem.Booking.ReceipientCustomerAddress.Location.Pincode,
                     Landmark = x.BookingItem.Booking.ReceipientCustomerAddress.Landmark,
                     IsPaid = x.BookingItem.Booking.PaidDate == null ? false : true,
                     Mobile = x.BookingItem.Booking.ReceipientCustomer.Mobile,
                     AlternativeMobile = x.BookingItem.Booking.ReceipientCustomerAddress.Mobile,
                     TotalBoxCount = x.BookingItem.Booking.Bookingitems.Sum(x => x.Quantity),
                     //PaymentType = mapPaymentType(x.Booking.PaymentMode)
                 })
                 .ToListAsync();
        }
        public void journeyStatusUpdate(int[] Ids)
        {
            var itemsToDeliver = _context.Journeyitems
                .Where(x => Ids.Any(i => i == x.Id))
                .ToList();
            var JourneyId = itemsToDeliver.Select(x => x.JourneyId).Distinct().SingleOrDefault();
            var Journeylist = _context.Journeys.Where(x => x.Id == JourneyId).SingleOrDefault();

            if (itemsToDeliver.Where(x=> x.Status == JourneyShipmentStatus.Delivered).Count() == itemsToDeliver.Count)
            {
                Journeylist.Status = JourneyStatus.Delivered;
            }
            if (itemsToDeliver.Where(x=> x.Status != JourneyShipmentStatus.Delivered).Count() > 0)
            {
                Journeylist.Status = JourneyStatus.PartiallyDelivered;
            }
        }
        private string mapPaymentType(string status)
        {
            switch (status)
            {
                case "S": return "Consignor"; break;
                case "C": return "Credit"; break;
                case "T": return "Consignee"; break;
                default: return "Undefined"; break;
            }
        }
    }

}
