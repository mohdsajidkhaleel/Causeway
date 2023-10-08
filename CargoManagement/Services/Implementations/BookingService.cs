using AutoMapper;
using CargoManagement.Models.Booking;
using CargoManagement.Models.JourneyItem;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace CargoManagement.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IEmailNotificationService _emailService;
        private readonly CMSConfig _config;
        public BookingService(IEmailNotificationService emailService, cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICustomerService customerService, IOptions<CMSConfig> config)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _customerService = customerService;
            _config = config.Value;
            _emailService = emailService;
        }
        public async Task<BookingResponseDTO> Create(BookingCreationDTO booking)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            //Preparing new booking object
            Booking newEntry = _mapper.Map<Booking>(booking);
            //newEntry.BookingId = RandomString(5);
            newEntry.BookingId = RandomNewString();
            newEntry.CreatedBy = userId;
            newEntry.OriginHubId = hubId;//Only a hub user should be able to create a new a booking
            newEntry.IsEmailNotificationSent = false;
            if (newEntry.PaymentMode.Equals(PaymentStatus.Self))
            {
                newEntry.PaidBy = newEntry.CustomerId;
                newEntry.TotalAmountReceived = newEntry.Bookingitems.Select(x => x.Quantity * x.UnitPrice).FirstOrDefault();
                newEntry.PaidDate = DateTime.UtcNow;
                newEntry.PaymentRemarks = "Consignor payment on booking";
            }
            else if (newEntry.PaymentMode.Equals(PaymentStatus.ToPay))
            {
                newEntry.PaymentRemarks = "Consignee payment on booking";
            }

            if (booking.isShipmentCollected)
            {
                newEntry.StatusId = BookingStatus.CollectedForShipment;
                newEntry.CurrentHubId = hubId;
                foreach (Bookingitem item in newEntry.Bookingitems)
                {
                    item.Bookingitemsdistributions.Add(new Bookingitemsdistribution() { BookingItem = item, HubId = hubId, Quantity = item.Quantity, DeliveredQty = 0, InTransitQty = 0, CreatedBy = userId });
                }
            }
            else
            {
                newEntry.StatusId = BookingStatus.New;//Shipment yet to collect from customer
            }
            //Transactions addition
            newEntry.Bookingtransactions.Add(new Bookingtransaction() { Booking = newEntry, CurrentHubId = hubId, StatusId = newEntry.StatusId, CreateDate = DateTime.UtcNow, CreatedBy = userId });

            //Adding and saving
            await _context.Bookings.AddAsync(newEntry);
            await _context.SaveChangesAsync();

            if (newEntry.Id != null)
            {
                Bookingpayment newPaymentEntry = new Bookingpayment
                {
                    BookingId = newEntry.Id,
                    BookingItemId = newEntry.Bookingitems.Select(x => x.Id).FirstOrDefault(),
                    JourneyId = null,
                    JourneyItemId = null,
                    PaymentMode = newEntry.PaymentMode,
                    TotalAmountToPay = newEntry.PaymentMode == "S" ? (newEntry.FreightCharges + (newEntry.Bookingitems.Select(x => x.UnitPrice * x.Quantity).FirstOrDefault())) : 0,
                    TotalAmountPaid = newEntry.PaymentMode == "S" ? (newEntry.FreightCharges + (newEntry.Bookingitems.Select(x => x.UnitPrice * x.Quantity).FirstOrDefault())) : 0,
                    IsPaymentCompleted = newEntry.PaymentMode == "S" ? true : false,
                    PayLaterBy = null,
                    TotalDispatchedQuantity = 0,
                    TotalQuantity = newEntry.Bookingitems.Select(x => x.Quantity).FirstOrDefault(),
                    CreatedDate = DateTime.UtcNow,
                    Paiddate = newEntry.PaymentMode == "S" ? DateTime.UtcNow : null,
                    Paidby = newEntry.PaymentMode == "S" ? newEntry.CustomerId : null,
                    AdditionalCharge = 0
                };
                await _context.Bookingpayments.AddAsync(newPaymentEntry);
                await _context.SaveChangesAsync();
            }
             await _emailService.CheckNewBookings();
            //Return response view
            return _mapper.Map<BookingResponseDTO>(newEntry);
        }

        public async Task<BookingResponseDTO> Update(BookingCreationDTO booking)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            Booking newEntry = _mapper.Map<Booking>(booking);
            var _existingBooking = _context.Bookings
                .Include(j => j.Bookingitems).ThenInclude(j => j.Bookingitemsdistributions)
                .Include(j => j.Bookingpayments)
                .SingleOrDefault(x => x.Id == newEntry.Id);

            var _existingJourney = _context.Journeyitems
                .FirstOrDefault(x => x.BookingItemId == newEntry.Bookingitems.FirstOrDefault().Id);

            if (_existingJourney != null)// checking whether journey is created, then updation not possible
            {
                return null;
            }
            if (_existingBooking != null) // updating table - Booking
            {
                _existingBooking.PaymentMode = newEntry.PaymentMode;
                _existingBooking.UpdatedBy = userId;
                _existingBooking.UpdatedDate = DateTime.UtcNow;

                _existingBooking.CustomerId = newEntry.CustomerId;
                _existingBooking.CustomerAddressId = newEntry.CustomerAddressId;
                _existingBooking.ReceipientCustomerId = newEntry.ReceipientCustomerId;
                _existingBooking.ReceipientCustomerAddressId = newEntry.ReceipientCustomerAddressId;
                _existingBooking.NextHubId = newEntry.NextHubId;
                _existingBooking.CurrentHubId = newEntry.CurrentHubId;
                _existingBooking.HandlingCharges = 0;
                _existingBooking.FreightCharges = 0;
                _existingBooking.RoundOffAmnount = 0;
                _existingBooking.Notes = newEntry.Notes;
                _existingBooking.ShipmentMode = newEntry.ShipmentMode;
                _existingBooking.IsEmailNotificationSent = false;
            }

            if (newEntry.PaymentMode.Equals(PaymentStatus.Self))
            {
                _existingBooking.PaidBy = newEntry.CustomerId;
                _existingBooking.PaidDate = DateTime.UtcNow;
                _existingBooking.PaymentRemarks = "Consignor payment on booking";
                _existingBooking.TotalAmount = newEntry.TotalAmount;
                _existingBooking.NetAmnount = newEntry.NetAmnount;

                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).PaymentMode = newEntry.PaymentMode;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).Paiddate = DateTime.UtcNow;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).Paidby = newEntry.CustomerId;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).TotalQuantity = newEntry.Bookingitems.Select(x => x.Quantity).FirstOrDefault();
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).IsPaymentCompleted = true;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).TotalAmountPaid = newEntry.FreightCharges + (newEntry.Bookingitems.Select(x => x.UnitPrice * x.Quantity).FirstOrDefault());
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).TotalAmountToPay = newEntry.FreightCharges + (newEntry.Bookingitems.Select(x => x.UnitPrice * x.Quantity).FirstOrDefault());
            }
            else if (newEntry.PaymentMode.Equals(PaymentStatus.ToPay))
            {
                _existingBooking.PaidBy = null;
                _existingBooking.PaidDate = null;
                _existingBooking.PaymentRemarks = "Consignee payment on booking";
                _existingBooking.TotalAmount = 0;
                _existingBooking.NetAmnount = 0;

                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).Paiddate = null;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).TotalQuantity = newEntry.Bookingitems.Select(x => x.Quantity).FirstOrDefault();
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).PaymentMode = newEntry.PaymentMode;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).IsPaymentCompleted = false;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).TotalAmountPaid = 0;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).TotalAmountToPay = 0;
                _existingBooking.Bookingpayments.FirstOrDefault(x => x.BookingId == newEntry.Id).Paidby = null;
            }

            if (booking.isShipmentCollected)
            {
                _existingBooking.StatusId = BookingStatus.CollectedForShipment;
                _existingBooking.CurrentHubId = hubId;

                foreach (Bookingitem item in _existingBooking.Bookingitems)
                {
                    var updatedItem = newEntry.Bookingitems.SingleOrDefault(x => x.Id == item.Id);
                    if (updatedItem != null)
                    {
                        //below updating table - BookingItem
                        item.Quantity = updatedItem.Quantity;
                        item.BoxTypeId = updatedItem.BoxTypeId;
                        item.UnitPrice = updatedItem.UnitPrice;
                        item.TotalPrice = updatedItem.TotalPrice;
                        item.Description = updatedItem.Description;
                        item.UpdatedDate = DateTime.UtcNow;
                        item.UpdatedBy = userId;

                        // belowupdating table - bookingitemsdistribution based on booking Id.
                        if (item.Bookingitemsdistributions != null)
                        {
                            var _itemdistribution = item.Bookingitemsdistributions.SingleOrDefault(x => x.BookingItemId == item.Id);
                            if (_itemdistribution != null)
                            {
                                _itemdistribution.Quantity = updatedItem.Quantity;
                                _itemdistribution.UpdatedDate = DateTime.UtcNow;
                                _itemdistribution.UpdatedBy = userId;
                            }
                        }
                    }
                }
            }
            else
            {
                newEntry.StatusId = BookingStatus.New;//Shipment yet to collect from customer
            }
            newEntry.Bookingtransactions.Add(new Bookingtransaction() { Booking = newEntry, CurrentHubId = hubId, StatusId = newEntry.StatusId, CreateDate = DateTime.UtcNow, CreatedBy = userId });

            _context.Update(_existingBooking);
            await _context.SaveChangesAsync();

            //Return response view
            return _mapper.Map<BookingResponseDTO>(newEntry);
        }

        public async Task<int> Delete(int Id)
        {
            var itemToRemove = _context.Bookings
                .Include(booking => booking.Bookingitems).ThenInclude(x => x.Bookingitemsdistributions)
                .Include(booking => booking.Bookingtransactions)
                .Include(booking => booking.Bookingpayments)
                .SingleOrDefault(x => x.Id == Id);

            if (itemToRemove != null)
            {
                if (itemToRemove.StatusId.Equals(BookingStatus.CollectedForShipment))
                {
                    itemToRemove.Bookingpayments.ToList().ForEach(x =>
                    {
                        _context.Remove<Bookingpayment>(x);
                    });
                    itemToRemove.Bookingitems.ToList().ForEach(x =>
                    {
                        x.Bookingitemsdistributions.ToList().ForEach(y =>
                        {
                            _context.Remove<Bookingitemsdistribution>(y);
                        });
                        _context.Remove<Bookingitem>(x);
                    });
                    itemToRemove.Bookingitems.ToList().ForEach(x =>
                    {
                        _context.Remove<Bookingitem>(x);
                    });
                    itemToRemove.Bookingtransactions.ToList().ForEach(x =>
                    {
                        _context.Remove<Bookingtransaction>(x);
                    });
                    _context.Remove<Booking>(itemToRemove);
                    _context.SaveChanges();

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
        public async Task<int> DeleteBookingItem(int Id)
        {
            var itemToRemove = _context.Bookingitems
                .Include(booking => booking.Bookingitemsdistributions)
                .Include(booking => booking.Journeyitems)
                .SingleOrDefault(x => x.Id == Id);

            if (itemToRemove != null)
            {
                if (itemToRemove.Journeyitems.Count == 0)
                {
                    itemToRemove.Bookingitemsdistributions.ToList().ForEach(y =>
                        {
                            _context.Remove<Bookingitemsdistribution>(y);
                        });
                    _context.Remove<Bookingitem>(itemToRemove);
                    _context.SaveChanges();
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

        public async Task<IEnumerable<BookingResponseDTO>> Get()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var list = await _context.Bookings
                .Include(booking => booking.Bookingitems)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingResponseDTO>>(list);
        }

        public IQueryable<BookingListDTO> GetList(BookingFilterDTO FilterList)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            DateTime? FromDate = FilterList.createdDate;
            DateTime? ToDate = FilterList.EndDate;
            FilterList.createdDate = FilterList.createdDate == null ? DateTime.MinValue : FilterList.createdDate;
            if (FilterList.EndDate == null)
            {
                if (FilterList.createdDate == DateTime.MinValue)
                {
                    FilterList.EndDate = DateTime.MaxValue;
                }
                else
                    FilterList.EndDate = FilterList.createdDate;
            }

            var list = _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.CustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.ReceipientCustomer)
                .Include(x => x.ReceipientCustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.CurrentHub)
                .Include(x => x.Status)
                .Include(x => x.Bookingitems)
                .Include(x => x.Journey)
                .OrderByDescending(x => x.CreateDate)
                .AsQueryable();

            if (list != null && (FromDate != null || ToDate != null))
            {
                if (FromDate != null && ToDate == null)
                {
                    list = list.Where(x => x.CreateDate.Value.Year == FilterList.createdDate.Value.Year && x.CreateDate.Value.Month == FilterList.createdDate.Value.Month && x.CreateDate.Value.Day == FilterList.createdDate.Value.Day).AsQueryable();
                   
                }
                else if (ToDate != null && FromDate == null)
                {
                    list = list.Where(x => x.CreateDate <= FilterList.EndDate).AsQueryable();
                }
                else
                {
                    list = list.Where(x => x.CreateDate >= FilterList.createdDate && x.CreateDate <= FilterList.EndDate).AsQueryable();
                }
            }
            var FilteredList = list;
            if (FilterList.Status != null)
            {
                FilteredList = FilteredList.Where(x => x.StatusId == FilterList.Status).AsQueryable();
            }
            if (FilterList.Mobile != null)
            {
                FilteredList = FilteredList.Where(x => x.Customer.Mobile.Contains(FilterList.Mobile.Trim()));
            }
            if (FilterList.IsClosed != null)
            {
                FilteredList = FilteredList.Where(x => x.IsClosed == FilterList.IsClosed).AsQueryable();
            }
            if (FilterList.BookingId != null)
            {
                FilteredList = FilteredList.Where(x => x.BookingId.Contains(FilterList.BookingId)).AsQueryable();
            }

            if (FilterList.ConsignorName != null)
            {
                FilteredList = FilteredList.Where(x => x.Customer.Name.Contains(FilterList.ConsignorName)).AsQueryable();
            }

            if (FilterList.ConsigneeName != null)
            {
                FilteredList = FilteredList.Where(x => x.ReceipientCustomer.Name.Contains(FilterList.ConsigneeName)).AsQueryable();
            }
            return _mapper.ProjectTo<BookingListDTO>(FilteredList);
        }

        public async Task<IEnumerable<BookingListDTO>> TodaysBookingExport(BookingFilterDTO FilterList)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            DateTime? todaysDate = FilterList.createdDate;

            var list = await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.CustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.ReceipientCustomer)
                .Include(x => x.ReceipientCustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.CurrentHub)
                .Include(x => x.Status)
                .Include(x => x.Bookingitems)
                .Include(x => x.Journey)
                .Where(x => x.CreateDate.Value.Year == todaysDate.Value.Year && x.CreateDate.Value.Month == todaysDate.Value.Month && x.CreateDate.Value.Day == todaysDate.Value.Day)                
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookingListDTO>>(list);
        }


        public IQueryable<BookingListDTO> GetDetailsAsQueryable()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var list = _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.CustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.ReceipientCustomer)
                .Include(x => x.ReceipientCustomerAddress).ThenInclude(address => address.Location).ThenInclude(location => location.District)
                .Include(x => x.CurrentHub)
                .Include(x => x.Status)
                .Include(x => x.Bookingitems)
                .Include(x => x.Journey)
                .OrderByDescending(x => x.CreateDate)
                .AsQueryable();
            Console.WriteLine(list.ToQueryString());
            return _mapper.ProjectTo<BookingListDTO>(list);
        }

        public async Task<bool?> ReceivePayment(int Id, int customerId, string paymentRemarks)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var booking = await _context.Bookings
                .SingleOrDefaultAsync(booking => booking.Id == Id);
            if (booking != null)
            {
                if (booking.PaidDate == null)
                {
                    booking.PaidDate = DateTime.UtcNow;
                    booking.PaidBy = customerId;
                    booking.PaymentRemarks = paymentRemarks;
                    booking.UpdatedBy = userId;

                    //Transactions addition
                    booking.Bookingtransactions.Add(new Bookingtransaction() { Booking = booking, StatusId = BookingStatus.CustomerPaymentCollected, CreateDate = DateTime.UtcNow, CreatedBy = userId, Remarks = paymentRemarks });

                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> ReturnBooking(int bookingId)
        {
            var booking = await _context.Bookingitems
                .Include(x => x.Booking)
                .Where(x => x.BookingId == bookingId)
                .SingleOrDefaultAsync();

            if (booking != null)
            {
                if (booking.PlannedQty > 0 || booking.InTransitQty > 0 || booking.ReceivedQty > 0 || booking.DeliveredQty > 0)
                {
                    return false;
                }
                else
                {
                    booking.Booking.StatusId = BookingStatus.ReturnedToConsignor;
                    booking.Booking.Notes = booking.Booking.Notes + " -> Returned Booking on : " + DateTime.UtcNow.ToString();
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }


        public async Task<BookingDetailsDTO> GetBookingDetails(int Id)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));

            var booking = await _context.Bookings
               .Include(book => book.Customer)
               .ThenInclude(cust => cust.Customeraddresses)
               .ThenInclude(address => address.Location).ThenInclude(loc => loc.District).ThenInclude(distr => distr.State)
                .Include(x => x.ReceipientCustomer).ThenInclude(cust => cust.Customeraddresses)
                .ThenInclude(address => address.Location).ThenInclude(loc => loc.District).ThenInclude(distr => distr.State)
                .Include(x => x.CurrentHub)
                .Include(x => x.OriginHub)
                .Include(x => x.Status)
                .Include(x => x.Journey)
                .Include(x => x.Bookingitems).ThenInclude(items => items.BoxType)
                .Include(x => x.Bookingitems).ThenInclude(y => y.Journeyitems).ThenInclude(x => x.Journey)
                .SingleOrDefaultAsync(x => x.Id == Id);

            BookingDetailsDTO GetBookingDetails = _mapper.Map<BookingDetailsDTO>(booking);
            if (GetBookingDetails != null)
            {
                if (GetBookingDetails.Sender.Address != null)
                {
                    GetBookingDetails.Sender.Address.RemoveAll(x => x.Id != booking.CustomerAddressId);
                }
                if (GetBookingDetails.Recepient.Address != null)
                {
                    GetBookingDetails.Recepient.Address.RemoveAll(x => x.Id != booking.ReceipientCustomerAddressId);
                }
            }
            return GetBookingDetails;
        }

        public async Task<bool> UpdateBookingStatus(int bookingId, int journeyId, string status, string comment, string? fileName)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var booking = _context.Bookings.FirstOrDefault(booking => booking.Id == bookingId);
            booking.Bookingtransactions.Add(new Bookingtransaction() { BookingId = bookingId, StatusId = status, JourneyId = journeyId, FileName = fileName, Remarks = comment, CreatedBy = userId });
            if (fileName != null)
            {
                booking.Bookingfiles.Add(new Bookingfile() { BookingId = bookingId, FileName = fileName, Remarks = comment, CreatedBy = userId });
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BookingFileDTO>> GetBookingFiles(int bookingId)
        {
            var files = await _context.Bookingfiles.Where(file => file.BookingId == bookingId).ToListAsync();
            var filesMapped = _mapper.Map<IEnumerable<BookingFileDTO>>(files);
            return filesMapped.Select(c => { c.Url = _config.FileDownloadUrl + "/" + _config.BookingDocsFolderAlias + "/" + c.FileName; return c; }).ToList();
        }
        public async Task<IEnumerable<DropDownDTO>> GetBookingCode()
        {
            return await _context.Bookings
                       .Select(x => new DropDownDTO
                       {
                           Text = x.BookingId,
                           Value = x.Id
                       })
                       .ToListAsync();
        }


        public async Task<IEnumerable<BookingTransactionsDTO>> GetBookingTransactions(int bookingId)
        {
            var history = await _context.Bookingtransactions
                .Include(history => history.Status)
                .Include(history => history.Journey)
                .Include(history => history.CurrentHub)
                .Include(history => history.OriginHub)
                .Include(history => history.NextHub)
                .Where(history => history.BookingId == bookingId).OrderByDescending(history => history.CreateDate).ToListAsync();
            return _mapper.Map<IEnumerable<BookingTransactionsDTO>>(history);
        }
        public async Task<IEnumerable<BookingStatusCodesDTO>> GetBookingStatusCodes()
        {
            return new List<BookingStatusCodesDTO> {
                    new BookingStatusCodesDTO { BookingStatusCode="NB",BookingStatusCodeName="New"},
                    new BookingStatusCodesDTO { BookingStatusCode="AP",BookingStatusCodeName="AssignedForPickUP"},
                    new BookingStatusCodesDTO { BookingStatusCode="PU",BookingStatusCodeName="PickedUp"},
                    new BookingStatusCodesDTO { BookingStatusCode="CS",BookingStatusCodeName="CollectedForShipment"},
                    new BookingStatusCodesDTO { BookingStatusCode="AT",BookingStatusCodeName="AssignForTransit"},
                    new BookingStatusCodesDTO { BookingStatusCode="IT",BookingStatusCodeName="InTransit"},
                    new BookingStatusCodesDTO { BookingStatusCode="AD",BookingStatusCodeName="AssignedForDelivery"},
                    new BookingStatusCodesDTO { BookingStatusCode="OD",BookingStatusCodeName="OutForDelivery"},
                    new BookingStatusCodesDTO { BookingStatusCode="CR",BookingStatusCodeName="ReturnedToCustomer"},
                    new BookingStatusCodesDTO { BookingStatusCode="DS",BookingStatusCodeName="DeliveredShipment"},
                    new BookingStatusCodesDTO { BookingStatusCode="CP",BookingStatusCodeName="CustomerPaymentCollected"},
                    new BookingStatusCodesDTO { BookingStatusCode="RJ",BookingStatusCodeName="RemovedFromJourney"},
                    new BookingStatusCodesDTO { BookingStatusCode="DH",BookingStatusCodeName="DeliverToHub"},
                    new BookingStatusCodesDTO { BookingStatusCode="PD",BookingStatusCodeName="PartiallyDelivered"}
                };
        }

        #region

        public async Task<bool> UpdateJourneyOnBooking(List<JourneyItemsCreationDTO> bookingIds, int journeyId)
        {
            foreach (JourneyItemsCreationDTO item in bookingIds)
            {
                var entity = new Booking() { Id = item.BookingItemId, JourneyId = journeyId };
                var attachedEntity = _context.Bookings.Attach(entity);
                entity.Bookingtransactions.Add(new Bookingtransaction() { BookingId = item.BookingItemId, StatusId = (item.Action == JourneyShipmentAction.Pickup ? BookingStatus.AssignedForPickUP.ToString() : BookingStatus.AssignedForDelivery.ToString()), JourneyId = journeyId, OriginHubId = item.OriginHubId, NextHubId = item.DestinationHubId });
                attachedEntity.Property(x => x.JourneyId).IsModified = true;
            }
            await _context.SaveChangesAsync();

            return true;
        }

        #endregion
        #region
        private string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            finalString = finalString + "-" + DateTime.UtcNow.ToString("ddMMyy-HHmm");
            return finalString;
        }

        private string RandomNewString()
        {
            string _defaultValue = _config.LastBookingId != null ? _config.LastBookingId : "0001";
            var _previousBookingNum = _context.Bookings
                .OrderByDescending(p => p.Id)
                .Select(x => x.BookingId)
                .FirstOrDefault();

            if (_previousBookingNum == null)
            {
                return _defaultValue;
            }
            if (int.TryParse(_previousBookingNum, out int value))
            {
                var _newBookingId = Convert.ToString(int.Parse(_previousBookingNum) + 1);
                if (int.Parse(_newBookingId) < 10)
                {
                    return _newBookingId = "000" + _newBookingId.ToString();
                }
                else if (int.Parse(_newBookingId) < 100)
                {
                    return _newBookingId = "00" + _newBookingId.ToString();
                }
                else if (int.Parse(_newBookingId) < 1000)
                {
                    return _newBookingId = "0" + _newBookingId.ToString();
                }
                else
                {
                    return _newBookingId = _newBookingId.ToString();
                }
            }
            else
                return _defaultValue;
        }

        public async Task<IEnumerable<ViewBookingStatusResponseDTO>> ViewBookingStatus(int bookingId)
        {
            var data = await _context.Bookingitems.Include(x => x.BoxType)
                 .Where(x => x.BookingId == bookingId)
                 .ToListAsync();
            return _mapper.Map<IEnumerable<ViewBookingStatusResponseDTO>>(data);
        }

        #endregion
    }
}
