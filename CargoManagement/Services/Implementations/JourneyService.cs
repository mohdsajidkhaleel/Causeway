using AutoMapper;
using CargoManagement.Models.Expense;
using CargoManagement.Models.Journey;
using CargoManagement.Models.JourneyItem;
using CargoManagement.Models.Shared;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Implementations
{
    public class JourneyService : IJourneyService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBookingService _bookingService;

        public JourneyService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBookingService bookingService)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _bookingService = bookingService;
        }
        public async Task<JourneyResponseDTO> Create(JourneyCreationDTO journey)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            //Preparing object for insert
            Journey newEntry = _mapper.Map<Journey>(journey);
            newEntry.CreatedBy = userId;
            newEntry.OriginHubId = newEntry.OriginHubId;
            newEntry.DestinationHubId = newEntry.DestinationHubId;
            newEntry.CreatorHubId = hubId;
            newEntry.Status = JourneyStatus.Scheduled;

            newEntry.Journeyitems.ToList().ForEach(x =>
            {
                x.OriginHubId = newEntry.OriginHubId;
                x.DestinationHubId = newEntry.DestinationHubId;
                x.ItemDistributionId = _context.Bookingitemsdistributions
                    .Where(y => y.BookingItemId == x.BookingItemId)
                    .Select(y => y.Id).SingleOrDefault();
            });
            //Adding and saving
            await _context.Journeys.AddAsync(newEntry);
            await _context.SaveChangesAsync();
            //Fetch journey details and update booking items and distribution

            var Journey = _context.Journeys
                .Include(j => j.Journeyitems).ThenInclude(i => i.BookingItem).ThenInclude(x => x.Booking)
                .Include(j => j.Journeyitems).ThenInclude(i => i.ItemDistribution)
                .SingleOrDefault(x => x.Id == newEntry.Id);

            foreach (Journeyitem item in Journey.Journeyitems)
            {
                item.PaymentMode = item.BookingItem.Booking.PaymentMode;
                //item.UnitPrice = item.BookingItem.UnitPrice;
                item.BookingItem.PlannedQty = item.BookingItem.PlannedQty + item.Quantity;
                item.Action = JourneyShipmentAction.Delivery;
                item.Status = JourneyShipmentStatus.Scheduled;
            }

            // below code is for update only consignor journey Details and dispatched quantity.
            var updatebookingPayment = await _context.Journeyitems
                .Include(x => x.BookingItem)
                .ThenInclude(x => x.Booking)
                .ThenInclude(x => x.Bookingpayments)
                .Where(x => x.JourneyId == newEntry.Id)
                .Where(x => x.BookingItem.Booking.PaymentMode == PaymentStatus.Self)
                .ToListAsync();


            foreach (var item in updatebookingPayment)
            {
                if (item.BookingItem.Booking.Bookingpayments.Count() == 1 &&
                    item.BookingItem.Booking.PaymentMode == PaymentStatus.Self &&
                    item.BookingItem.Booking.Bookingpayments.Any(x => x.JourneyId == null))
                {
                    if (item.BookingItem.Booking.Bookingpayments.FirstOrDefault().JourneyId == null)
                    {
                        item.BookingItem.Booking.Bookingpayments.FirstOrDefault().JourneyId = item.JourneyId;
                        item.BookingItem.Booking.Bookingpayments.FirstOrDefault().JourneyItemId = item.Id;
                        item.BookingItem.Booking.Bookingpayments.FirstOrDefault().TotalDispatchedQuantity = item.Quantity;
                    }
                }
                else if (item.BookingItem.Booking.PaymentMode == PaymentStatus.Self &&
                    item.BookingItem.Booking.Bookingpayments.Any(x => x.JourneyId != null))
                {
                    Bookingpayment newPaymentEntry = new Bookingpayment
                    {
                        Paiddate = item.BookingItem.Booking.PaidDate,
                        Paidby = item.BookingItem.Booking.PaidBy,
                        BookingId = item.BookingItem.Booking.Id,
                        BookingItemId = item.BookingItem.Id,
                        JourneyId = item.JourneyId,
                        JourneyItemId = item.Id,
                        AdditionalCharge = 0,
                        TotalAmountToPay = item.BookingItem.UnitPrice * item.BookingItem.Quantity,
                        TotalAmountPaid = item.BookingItem.UnitPrice * item.BookingItem.Quantity,
                        Discount = 0,
                        IsPaymentCompleted = true,
                        PayLaterBy = null,
                        TotalDispatchedQuantity = item.Quantity,
                        TotalQuantity = item.BookingItem.Quantity,
                        CreatedDate = DateTime.UtcNow,
                        PaymentMode = PaymentStatus.Self
                    };
                    await _context.Bookingpayments.AddAsync(newPaymentEntry);
                }
            }

            await _context.SaveChangesAsync();
            //Return response view
            return _mapper.Map<JourneyResponseDTO>(newEntry);
        }

        public async Task<int> Update(JourneyCreationDTO journey)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            Journey newJourney = _mapper.Map<Journey>(journey);
            var ExistingJourney = _context.Journeys
                .Include(j => j.Journeyitems).ThenInclude(i => i.BookingItem)
                .Include(j => j.Journeyitems).ThenInclude(i => i.ItemDistribution)
                .SingleOrDefault(x => x.Id == newJourney.Id);

            ExistingJourney.DateOfJourney = newJourney.DateOfJourney;
            ExistingJourney.Notes = newJourney.Notes;

            if (ExistingJourney.Status.Equals(JourneyStatus.Scheduled))
            {
                ExistingJourney.Name = newJourney.Name;
                ExistingJourney.ContainerId = newJourney.ContainerId;

                foreach (Journeyitem item in ExistingJourney.Journeyitems)
                {
                    var updatedItem = newJourney.Journeyitems.FirstOrDefault(x => x.Id == item.Id);
                    if (updatedItem != null)
                    {
                        if (item.Quantity > updatedItem.Quantity)
                        {
                            var diff = item.Quantity - updatedItem.Quantity;
                            item.BookingItem.PlannedQty = item.BookingItem.PlannedQty - diff;
                        }
                        else if (item.Quantity < updatedItem.Quantity)
                        {
                            var diff = updatedItem.Quantity - item.Quantity;
                            item.BookingItem.PlannedQty = item.BookingItem.PlannedQty + diff;
                        }
                        item.DestinationHubId = newJourney.DestinationHubId;
                        item.OriginHubId = newJourney.OriginHubId;
                        item.Quantity = updatedItem.Quantity;
                        item.Notes = updatedItem.Notes;                        
                        item.Action = item.Action;
                        item.UpdatedBy = userId;
                        item.ItemDistributionId = _context.Bookingitemsdistributions
                             .Where(y => y.BookingItemId == item.BookingItemId)
                             .Select(y => y.Id)
                             .FirstOrDefault();
                    }
                }

                foreach (Journeyitem item in newJourney.Journeyitems.Where(x => x.Id == 0))
                {
                    item.DestinationHubId = newJourney.DestinationHubId;
                    item.OriginHubId = newJourney.OriginHubId;
                    item.Status = JourneyShipmentStatus.Scheduled;
                    var distribution = _context.Bookingitemsdistributions
                     .Include(x => x.BookingItem)
                     .Where(y => y.BookingItemId == item.BookingItemId)
                     .SingleOrDefault();
                    item.ItemDistributionId = distribution.Id;
                    distribution.BookingItem.PlannedQty = distribution.BookingItem.PlannedQty + item.Quantity;
                    ExistingJourney.Journeyitems.Add(item);
                }

                if (ExistingJourney.Journeyitems.Where(x => x.Id == 0).Count() > 0)
                {

                }

                await _context.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public async Task<int> Delete(int Id)
        {
            var itemToRemove = _context.Journeys
                .Include(journey => journey.Journeyitems).ThenInclude(x => x.BookingItem)
                .SingleOrDefault(x => x.Id == Id);

            if (itemToRemove != null)
            {
                if (itemToRemove.Status.Equals(JourneyStatus.Scheduled))
                {
                    itemToRemove.Journeyitems.ToList().ForEach(x =>
                    {
                        x.BookingItem.PlannedQty = x.BookingItem.PlannedQty - x.Quantity;
                        _context.Remove<Journeyitem>(x);
                    });
                    _context.Remove<Journey>(itemToRemove);
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

        public async Task<bool> JourneyPayment(JourneyPaymentDTO paymentList)
        {
            var _mainDetails = await _context.Journeyitems
            .Include(x => x.BookingItem).ThenInclude(x => x.Booking)
            .Where(y => y.Id == paymentList.Id && y.JourneyId == paymentList.JourneyId)
            .SingleOrDefaultAsync();

            var _bookingDetails = await _context.Bookingitems
                .Include(x => x.Booking)
                .Where(x => x.Id == paymentList.BookingItemId)
                .SingleOrDefaultAsync();

            if (_mainDetails != null)
            {
                if (_mainDetails.Status.Equals(JourneyShipmentStatus.Received) &&
                    _mainDetails.PaymentMode.Equals(PaymentStatus.ToPay) &&
                    _mainDetails.BookingItem.UnitPrice > 0)
                {
                    if ((paymentList.CustomerId == _bookingDetails.Booking.CustomerId) ||
                        (paymentList.CustomerId == _bookingDetails.Booking.ReceipientCustomerId))
                    //above if condition is for checking the paylater is only Consignor or consignee
                    {
                        var _paymentTableData = await _context.Bookingpayments
                             .Where(x => x.BookingId == _mainDetails.BookingItem.BookingId)
                             .ToListAsync();

                        if (_paymentTableData.Count() == 1 && _paymentTableData.Select(x => x.JourneyId).FirstOrDefault() == null)
                        {
                            var _updatePayment = await _context.Bookingpayments
                            .Where(x => x.BookingId == _mainDetails.BookingItem.BookingId)
                            .SingleOrDefaultAsync();

                            _updatePayment.Paiddate = DateTime.UtcNow;
                            _updatePayment.Paidby = _mainDetails.BookingItem.Booking.ReceipientCustomerId;
                            _updatePayment.JourneyId = _mainDetails.JourneyId;
                            _updatePayment.JourneyItemId = _mainDetails.Id;
                            _updatePayment.TotalAmountToPay = _mainDetails.BookingItem.UnitPrice * _mainDetails.BookingItem.Quantity;
                            _updatePayment.TotalAmountPaid = _mainDetails.Quantity * _mainDetails.BookingItem.UnitPrice;
                            _updatePayment.IsPaymentCompleted = _updatePayment.TotalAmountToPay == _updatePayment.TotalAmountPaid ? true : false;
                            _updatePayment.PayLaterBy = null;
                            _updatePayment.TotalDispatchedQuantity = _mainDetails.Quantity;
                            _updatePayment.TotalQuantity = _mainDetails.BookingItem.Quantity;

                            if (_updatePayment.IsPaymentCompleted == true)
                            {
                                _mainDetails.BookingItem.Booking.PaidBy = _mainDetails.BookingItem.Booking.ReceipientCustomerId;
                                _mainDetails.BookingItem.Booking.PaidDate = DateTime.UtcNow;
                                _mainDetails.BookingItem.Booking.PayLaterBy = null;
                                _mainDetails.BookingItem.Booking.TotalAmountReceived = _mainDetails.BookingItem.Booking.TotalAmountReceived + (
                                    _mainDetails.Quantity * _mainDetails.BookingItem.UnitPrice);
                            }
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            Bookingpayment newPaymentEntry = new Bookingpayment
                            {
                                Paiddate = DateTime.UtcNow,
                                Paidby = _mainDetails.BookingItem.Booking.ReceipientCustomerId,
                                BookingId = _mainDetails.BookingItem.Booking.Id,
                                BookingItemId = _mainDetails.BookingItemId,
                                JourneyId = _mainDetails.JourneyId,
                                JourneyItemId = _mainDetails.Id,
                                TotalAmountToPay = _mainDetails.BookingItem.UnitPrice * _mainDetails.BookingItem.Quantity,
                                TotalAmountPaid = _mainDetails.Quantity * _mainDetails.BookingItem.UnitPrice,
                                IsPaymentCompleted = _mainDetails.BookingItem.UnitPrice * _mainDetails.BookingItem.Quantity == (_paymentTableData.Sum(x => x.TotalAmountPaid)
                                                   + (_mainDetails.Quantity * _mainDetails.BookingItem.UnitPrice)) ? true : false,
                                PayLaterBy = null,
                                TotalDispatchedQuantity = _mainDetails.Quantity,
                                TotalQuantity = _mainDetails.BookingItem.Quantity,
                                CreatedDate = DateTime.UtcNow,
                                PaymentMode = PaymentStatus.ToPay
                            };
                            if (newPaymentEntry.IsPaymentCompleted == true)
                            {
                                _mainDetails.BookingItem.Booking.PaidBy = _mainDetails.BookingItem.Booking.ReceipientCustomerId;
                                _mainDetails.BookingItem.Booking.PaidDate = DateTime.UtcNow;
                                _mainDetails.BookingItem.Booking.PayLaterBy = null;
                                _mainDetails.BookingItem.Booking.TotalAmountReceived = _mainDetails.BookingItem.Booking.TotalAmountReceived + (
                                    _mainDetails.Quantity * _mainDetails.BookingItem.UnitPrice);
                            }
                            await _context.Bookingpayments.AddAsync(newPaymentEntry);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            return false;
        }

        public IQueryable<JourneyResponseDTO> Get(JourneyFilterDTO FilterList)
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
            var listdata =  _context.Journeys
                .Include(x => x.Journeyitems).ThenInclude(x => x.BookingItem).ThenInclude(x => x.Booking)
                .Include(x => x.Journeyitems).ThenInclude(x => x.BookingItem).ThenInclude(x => x.Bookingitemsdistributions)
                .Include(x => x.DestinationHub)
                //.Where(x => x.OriginHubId == (hubId != null ? hubId : x.OriginHubId))
                .OrderByDescending(x => x.CreateDate)
                .ToList();

            List<JourneyResponseDTO> updatedList = _mapper.Map<List<JourneyResponseDTO>>(listdata);           

            foreach (var _journeyData in updatedList)
            {
                foreach (var _journeyItemData in _journeyData.Items)
                {
                    if (_journeyItemData.PaymentMode == PaymentStatus.ToPay && _journeyItemData.IsPaymentSuccessfull == null)
                    {
                        var jour_ID = _context.Bookingpayments
                            .Where(x => x.JourneyId == _journeyItemData.JourneyId &&
                            x.BookingId == _journeyItemData.BookingId)
                            .Select(x => x.JourneyId)
                            .FirstOrDefault();

                        _journeyItemData.IsPaymentSuccessfull = jour_ID == null ? false : true;
                    }
                }
            }


            if (updatedList != null && (FromDate != null || ToDate != null))
            {
                if (FromDate != null && ToDate == null)
                {
                    updatedList = updatedList.Where(x => x.CreateDate.Value.Year == FilterList.createdDate.Value.Year && x.CreateDate.Value.Month == FilterList.createdDate.Value.Month && x.CreateDate.Value.Day == FilterList.createdDate.Value.Day)
                   .ToList();
                }
                else if (ToDate != null && FromDate == null)
                {
                    updatedList = updatedList.Where(x => x.CreateDate <= FilterList.EndDate).ToList();
                }
                else
                {
                    updatedList = updatedList.Where(x => x.CreateDate >= FilterList.createdDate && x.CreateDate <= FilterList.EndDate).ToList();
                }
            }
            //var FilteredList = updatedList;
            if (FilterList.Status != null)
            {
                updatedList = updatedList.Where(x => x.Status == FilterList.Status).ToList();
            }
            if (FilterList.DateOfJourney != null)
            {
                updatedList = updatedList.Where(x => x.DateOfJourney.Year == FilterList.DateOfJourney.Value.Year &&
                    x.DateOfJourney.Month == FilterList.DateOfJourney.Value.Month &&
                    x.DateOfJourney.Day == FilterList.DateOfJourney.Value.Day)
                   .ToList();
            }
            return updatedList.AsQueryable();
        }

        public async Task<IEnumerable<JourneyListDTO>> GetList()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            List<Journey> list = await _context.Journeys
                .Include(x => x.OriginHub)
                .Include(x => x.DestinationHub)
                .Include(x => x.Journeyitems)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
            List<JourneyListDTO> journeyList = _mapper.Map<List<JourneyListDTO>>(list);
            return journeyList;
        }

        public IQueryable<JourneyListDTO> GetDetailsAsQueryable()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var list = _context.Journeys
                  .Include(x => x.OriginHub)
                  .Include(x => x.DestinationHub)
                  .Include(x => x.Journeyitems)
                  .OrderByDescending(x => x.CreateDate)
                  .AsQueryable();
            return _mapper.ProjectTo<JourneyListDTO>(list);
        }

        public IQueryable<JourneyListDTO> GetScheduledDetailsAsQueryable()
        {
            var list = _context.Journeys
                  .Include(x => x.OriginHub)
                  .Include(x => x.DestinationHub)
                  .Include(x => x.Journeyitems)
                  .Where(x => x.Status != JourneyStatus.Cancelled && x.Status != JourneyStatus.Ended && x.IsLocal == false)
                  .OrderBy(x => x.DateOfJourney)
                  .AsQueryable();
            return _mapper.ProjectTo<JourneyListDTO>(list);
        }

        public async Task<JourneyResponseDTO> GetById(int Id)
        {
            var journey = await _context.Journeys
                .Include(x => x.Journeyitems).ThenInclude(item => item.BookingItem.Booking)
                .Include(x => x.Journeyitems).ThenInclude(item => item.DestinationHub)
                .FirstOrDefaultAsync(x => x.Id == Id);
            return _mapper.Map<JourneyResponseDTO>(journey);
        }

        public async Task<bool> CancelJourney(int Id)
        {
            try
            {
                var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

                //getting details of the journey
                var journey = await _context.Journeys
                   .Include(j => j.Journeyitems).ThenInclude(i => i.BookingItem).ThenInclude(j => j.Booking)
                    .Include(j => j.Journeyitems).ThenInclude(i => i.ItemDistribution)
                   .SingleOrDefaultAsync(x => x.Id == Id);
                if (journey != null && journey.Status == JourneyStatus.InTransit)
                {
                    //Journey updating as ended
                    journey.Status = JourneyStatus.Cancelled;
                    journey.UpdatedBy = userId;

                    foreach (Journeyitem item in journey.Journeyitems)
                    {
                        item.ItemDistribution.InTransitQty = item.ItemDistribution.InTransitQty - item.Quantity;
                        item.BookingItem.InTransitQty = item.BookingItem.InTransitQty - item.Quantity;
                        item.BookingItem.PlannedQty = item.BookingItem.PlannedQty - item.Quantity;
                        item.Status = JourneyShipmentStatus.Cancelled;
                        item.BookingItem.Booking.StatusId = BookingStatus.RemovedFromJourney;
                    }

                    //Updating changes to repo
                    _context.Update(journey);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        public async Task<IEnumerable<JourneyExpenseCreationDTO>> CreateJourneyExpense(List<JourneyExpenseCreationDTO> expenses)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            //Preparing object for insert
            List<Journeyexpense> newEntries = _mapper.Map<List<Journeyexpense>>(expenses);
            newEntries = newEntries.Select(c => { c.CreatedBy = userId; return c; }).ToList();
            await _context.Journeyexpenses.AddRangeAsync(newEntries);
            await _context.SaveChangesAsync();

            //Return response view
            return _mapper.Map<List<JourneyExpenseCreationDTO>>(newEntries);
        }
        public async Task<IEnumerable<JourneyExpenseCreationDTO>> UpdateExpenses(List<JourneyExpenseCreationDTO> expenses)
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            //Preparing object for insert
            List<Journeyexpense> newEntries = _mapper.Map<List<Journeyexpense>>(expenses);
            newEntries = newEntries.Select(c => { c.UpdatedBy = userId; c.UpdatedDate = DateTime.UtcNow; return c; }).ToList();

            newEntries.ForEach(exp =>
            {
                var existData = _context.Journeyexpenses.Where(x => x.Id == exp.Id).SingleOrDefault();
                existData.JourneyId = exp.JourneyId;
                existData.Amount = exp.Amount;
                existData.Notes = exp.Notes;
                existData.ExpenseTypeId = exp.ExpenseTypeId;
            });
            await _context.SaveChangesAsync();
            return _mapper.Map<List<JourneyExpenseCreationDTO>>(newEntries);
        }
        public async Task<IEnumerable<GetJourneyforUnitPriceDTO>> GetJourneyDetailsUnitPrice(int journeyId)
        {
            return await _context.Journeyitems
                .Include(x => x.Journey)
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.Customer)
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.ReceipientCustomer)
                .Include(x => x.BookingItem).ThenInclude(x => x.BoxType)
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.Bookingpayments)
                .Where(x => x.JourneyId == journeyId)
                .Select(x => new GetJourneyforUnitPriceDTO
                {
                    JourneyId = x.JourneyId,
                    BookingItemNotes = x.BookingItem.Description,
                    JourneyItemId = x.Id,
                    JourneyName = x.Journey.Name,
                    BookingItemId = x.BookingItemId,
                    BookingCode = x.BookingItem.Booking.BookingId,
                    BookingId = x.BookingItem.BookingId,
                    ConsignorName = x.BookingItem.Booking.Customer.Name,
                    ConsigneeName = x.BookingItem.Booking.ReceipientCustomer.Name,
                    TotalQuantity = x.BookingItem.Quantity,
                    DispatchedQuantity = x.Quantity,
                    UnitPrice = x.BookingItem.UnitPrice,
                    Boxtype = x.BookingItem.BoxType.Name,
                    IsEditable = x.BookingItem.Booking.StatusId == PaymentStatus.Self ? false :
                                 x.BookingItem.Booking.Bookingpayments
                                              .Where(y => y.TotalDispatchedQuantity > 0 && y.BookingId == x.BookingItem.BookingId)
                                              .Count() > 0 ? false :
                                 x.BookingItem.DeliveredQty > 0 ? false : true
                }).ToListAsync();

        }

        public async Task<bool> IsUnitPriceEditable(int BookingId)
        {
            var bookingDetails = await _context.Bookings
                .Include(x => x.Bookingitems)
                .Include(x => x.Bookingpayments)
                .Where(x => x.Id == BookingId)
                .SingleOrDefaultAsync();

            return false;
        }


        public async Task<bool> DeleteExpenses(int Id)
        {
            var itemToRemove = await _context.Journeyexpenses
                .Where(x => x.Id == Id).SingleOrDefaultAsync();

            if (itemToRemove != null)
            {
                _context.Remove<Journeyexpense>(itemToRemove);
                await _context.SaveChangesAsync();

            }
            return true;
        }
        public async Task<bool> UpdateUnitPrice(JourneyUpdateUnitPriceDTO UnitPriceList)
        {
            var DataList = await _context.Journeyitems
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking)
                .Where(x => x.JourneyId == UnitPriceList.JourneyId)
                .ToListAsync();

            if (DataList != null)
            {
                foreach (var item in DataList)
                {
                    var newData = UnitPriceList.UnitPriceDetails
                        .Where(x => x.BookingItemId == item.BookingItemId)
                        .SingleOrDefault();
                    item.BookingItem.UnitPrice = newData.UnitPrice;
                    item.BookingItem.TotalPrice = (newData.UnitPrice * item.BookingItem.Quantity) + item.BookingItem.Booking.FreightCharges;
                    item.BookingItem.Booking.TotalAmount = item.BookingItem.TotalPrice;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<int> UpdatePayLaterBy(PayLaterDetailsDTO PayLaterDetails)
        {

            var _data = await _context.Journeyitems
            .Include(x => x.BookingItem).ThenInclude(x => x.Booking)
            .Where(x => x.Id == PayLaterDetails.JourneyItemId && x.JourneyId == PayLaterDetails.JourneyId)
            .SingleOrDefaultAsync();

            var _bookingDetails = await _context.Bookings
                .Where(x => x.Id == PayLaterDetails.BookingId)
                .SingleOrDefaultAsync();

            var _customerData = await _context.Customers
                .Where(x => x.Id == PayLaterDetails.PayLaterCustomerId)
                .SingleOrDefaultAsync();

            if (_data != null && _customerData != null && _data.BookingItem.Booking.PaymentMode == "T")
            {
                if (_data.BookingItem.UnitPrice > 0)
                {
                    if ((PayLaterDetails.PayLaterCustomerId == _bookingDetails.CustomerId) ||
                        (PayLaterDetails.PayLaterCustomerId == _bookingDetails.ReceipientCustomerId))
                    //above if condition is for checking the paylater is only Consignor or consignee
                    {
                        _customerData.OutstandingCredit = _customerData.OutstandingCredit + (_data.Quantity * _data.BookingItem.UnitPrice) + _data.BookingItem.Booking.FreightCharges;
                        _data.BookingItem.Booking.PayLaterBy = PayLaterDetails.PayLaterCustomerId;

                        var _paymentTableData = await _context.Bookingpayments
                            .Where(x => x.BookingId == PayLaterDetails.BookingId)
                            .ToListAsync();

                        if (_paymentTableData.Count() == 1 && _paymentTableData.Select(x => x.JourneyId).FirstOrDefault() == null)
                        {
                            var _updatePayment = await _context.Bookingpayments
                            .Where(x => x.BookingId == PayLaterDetails.BookingId)
                            .SingleOrDefaultAsync();

                            _updatePayment.Paidby = null;
                            _updatePayment.Paiddate = null;
                            _updatePayment.JourneyId = _data.JourneyId;
                            _updatePayment.JourneyItemId = _data.Id;
                            _updatePayment.TotalAmountToPay = _data.BookingItem.UnitPrice * _data.BookingItem.Quantity;
                            _updatePayment.TotalAmountPaid = 0;
                            _updatePayment.IsPaymentCompleted = false;
                            _updatePayment.PayLaterBy = PayLaterDetails.PayLaterCustomerId;
                            _updatePayment.TotalDispatchedQuantity = _data.Quantity;
                            _updatePayment.TotalQuantity = _data.BookingItem.Quantity;

                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            Bookingpayment newPaymentEntry = new Bookingpayment
                            {
                                BookingId = _data.BookingItem.Booking.Id,
                                BookingItemId = _data.BookingItemId,
                                JourneyId = _data.JourneyId,
                                JourneyItemId = _data.Id,
                                TotalAmountToPay = _data.BookingItem.UnitPrice * _data.BookingItem.Quantity,
                                TotalAmountPaid = 0,
                                IsPaymentCompleted = false,
                                PayLaterBy = PayLaterDetails.PayLaterCustomerId,
                                TotalDispatchedQuantity = _data.Quantity,
                                TotalQuantity = _data.BookingItem.Quantity,
                                CreatedDate = DateTime.UtcNow,
                                PaymentMode = PaymentStatus.ToPay,
                                Paidby = null,
                                Paiddate = null
                            };
                            await _context.Bookingpayments.AddAsync(newPaymentEntry);
                        }
                        await _context.SaveChangesAsync();
                        return 1;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    return 2;
                }
            }
            return 0;
        }



        public IQueryable<GetExpenseResponseDTO> GetExpenseList(int? journeyId)
        {
            if (journeyId != null && journeyId > 0) // to get journey expense list
            {
                var hubs = _context.Journeyexpenses
                    .Include(x => x.ExpenseType)
                    .Where(x => x.JourneyId == journeyId)
                    .OrderByDescending(x => x.CreateDate)
               .AsQueryable();
                return _mapper.ProjectTo<GetExpenseResponseDTO>(hubs);
            }
            else if (journeyId == null) // to get office expense list
            {
                var hubs =  _context.Journeyexpenses
                    .Include(x => x.ExpenseType)
                    .Where(x => x.JourneyId == null)
                    .OrderByDescending(x => x.CreateDate)
               .AsQueryable();
                return _mapper.ProjectTo<GetExpenseResponseDTO>(hubs);
            }
            else if (journeyId == 0) // to get full expense list
            {
                var hubs = _context.Journeyexpenses
                    .Include(x => x.ExpenseType)
                    .OrderByDescending(x => x.CreateDate)
               .AsQueryable();
                return _mapper.ProjectTo<GetExpenseResponseDTO>(hubs);
            }
            return null;

        }

        public async Task<IEnumerable<JourneyStartExportDetailsDTO>> GetTodaysJourneyStartDetails(ManifestReportFilterDTO filterdata)
        {
            var filterJourneyIds = await _context.Journeys
                .Where(x => x.DateOfJourney.Year == filterdata.DateofDispatch.Year && x.DateOfJourney.Month == filterdata.DateofDispatch.Month && x.DateOfJourney.Day == filterdata.DateofDispatch.Day)
                .Where(x => x.Id == filterdata.JourneyID)
                .Select(x => x.Id)
                .ToArrayAsync();

            if (filterJourneyIds.Count() > 0)
            {
                var _fullList = await _context.Journeyitems
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.Customer)
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.ReceipientCustomer)
                .Include(x => x.Journey)
                .Where(x => filterJourneyIds.Any(y => y == x.JourneyId))
                .OrderByDescending(x => x.BookingItem.Booking.BookingId)
                .ToListAsync();

                if (_fullList != null)
                {
                    return _fullList.Select(x => new JourneyStartExportDetailsDTO
                    {
                        JourneyDate = x.Journey.DateOfJourney,
                        JourneyName = x.Journey.Name,
                        JourneyId = x.JourneyId,
                        JourneyItemId = x.Id,
                        Packages = x.Quantity,
                        BookingId = x.BookingItem.Booking.BookingId,
                        CollectionCharges = 0,
                        ConsigneeName = x.BookingItem.Booking.ReceipientCustomer.Name,
                        ConsignorName = x.BookingItem.Booking.Customer.Name,
                        ContactNumber = x.BookingItem.Booking.ReceipientCustomer.Mobile,
                        ItemName = x.BookingItem.Description,
                        Notes = x.BookingItem.Booking.Notes
                    });
                }
            }
            return null;
        }

        #region  End points  for mobile
        public async Task<bool> StartJourney(int Id)
        {
            try
            {
                var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

                var journey = await _context.Journeys
                .Include(j => j.Journeyitems).ThenInclude(i => i.BookingItem).ThenInclude(k => k.Booking)
                .Include(j => j.Journeyitems).ThenInclude(i => i.BookingItem).ThenInclude(z => z.Bookingitemsdistributions)
                .SingleOrDefaultAsync(x => x.Id == Id);

                if (journey != null && journey.Status == JourneyStatus.Scheduled)
                {
                    journey.Status = JourneyStatus.InTransit;
                    journey.UpdatedBy = userId;

                    //Update journey status to in progress and booking status to in transit for those are deliverables

                    //.Where(c => c.Action == JourneyShipmentAction.Delivery)
                    //&& c.BookingItem.Booking.CurrentHubId == c.OriginHubId
                    journey.Journeyitems.ToList()
                    .ForEach(x =>
                    {
                        x.BookingItem.Booking.StatusId = BookingStatus.InTransit;
                        x.BookingItem.Booking.CurrentHubId = null;
                        x.BookingItem.Booking.NextHubId = x.DestinationHubId != null ? x.DestinationHubId : null;
                        x.BookingItem.Booking.UpdatedBy = userId;
                        x.Status = JourneyStatus.InTransit;

                        if (x.BookingItem != null)
                        {
                            x.BookingItem.PlannedQty = x.BookingItem.PlannedQty - x.Quantity;
                            x.BookingItem.InTransitQty = x.BookingItem.InTransitQty + x.Quantity;
                            x.BookingItem.UpdatedBy = userId;
                            x.UpdatedDate = DateTime.UtcNow;
                        }

                        if (x.BookingItem.Bookingitemsdistributions != null)
                        {
                            x.BookingItem.Bookingitemsdistributions.Where(y => y.BookingItemId == x.BookingItemId).ToList()
                            .ForEach((y) =>
                            {
                                y.InTransitQty = y.InTransitQty + x.Quantity;
                                y.UpdatedBy = userId;
                                y.UpdatedDate = DateTime.UtcNow;
                            });
                        }
                        x.BookingItem.Booking.Bookingtransactions.Add(new Bookingtransaction() { BookingId = x.BookingItem.Booking.Id, StatusId = BookingStatus.PickedUp, CreatedBy = userId });
                    });
                    _context.Update(journey);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        public async Task<bool> EndJourney(int Id)
        {
            try
            {
                var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

                //getting details of the journey
                var journey = await _context.Journeys
                .Include(j => j.Journeyitems).ThenInclude(i => i.BookingItem).ThenInclude(k => k.Booking)
                .Include(j => j.Journeyitems).ThenInclude(i => i.BookingItem).ThenInclude(z => z.Bookingitemsdistributions)
                .SingleOrDefaultAsync(x => x.Id == Id);

                if (journey != null && journey.Status == JourneyStatus.InTransit)
                {
                    //Journey updating as ended
                    journey.Status = JourneyStatus.Ended;
                    journey.UpdatedBy = userId;

                    //Updating picked up items and un delivered items to destination hub
                    journey.Journeyitems
                    .Where(c => c.Status == JourneyShipmentStatus.InTransit)
                    .ToList()
                    .ForEach(x =>
                    {
                        //Booking updates
                        x.BookingItem.Booking.NextHubId = null;
                        x.BookingItem.Booking.CurrentHubId = journey.DestinationHubId;
                        x.BookingItem.Booking.UpdatedBy = userId;
                        x.BookingItem.Booking.JourneyId = x.JourneyId;
                        x.BookingItem.Booking.StatusId = BookingStatus.DeliverToHub;

                        //BookingItem Update
                        x.BookingItem.InTransitQty = x.BookingItem.InTransitQty - x.Quantity;
                        x.BookingItem.ReceivedQty = x.BookingItem.ReceivedQty + x.Quantity;
                        x.UpdatedBy = userId;
                        x.UpdatedDate = DateTime.UtcNow;
                        //journey items update
                        x.Status = JourneyShipmentStatus.Received;

                        //BookingItemDistribution Update
                        if (x.BookingItem.Bookingitemsdistributions != null)
                        {
                            x.BookingItem.Bookingitemsdistributions.ToList()
                                                    .ForEach((y) =>
                                                    {
                                                        y.InTransitQty = y.InTransitQty - x.Quantity;
                                                        y.ReceivedQty = y.ReceivedQty + x.Quantity;
                                                        y.UpdatedBy = userId;
                                                        y.UpdatedDate = DateTime.UtcNow;
                                                    });
                        }
                        //Transaction History
                        x.BookingItem.Booking.Bookingtransactions.Add(new Bookingtransaction() { BookingId = x.BookingItem.Booking.Id, StatusId = BookingStatus.DeliverToHub, CurrentHubId = journey.DestinationHubId, CreatedBy = userId });
                    });
                    //Updating changes to repo
                    _context.Update(journey);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
        public async Task<IEnumerable<JourneyListDTO>> GetMyJourneyList()
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            List<Journey> list = await _context.Journeys
                .Include(x => x.OriginHub)
                .Include(x => x.DestinationHub)
                .Include(x => x.Journeyitems.Where(items => items.Status != JourneyShipmentStatus.Delivered))
                .Where(x => x.DriverId == userId && x.Status != JourneyStatus.Ended && x.Status != JourneyStatus.Cancelled)
                .OrderBy(x => x.DateOfJourney)
                .ToListAsync();
            List<JourneyListDTO> journeyList = _mapper.Map<List<JourneyListDTO>>(list);
            return journeyList;
        }
        public async Task<JourneyDeliveryNoteDTO> GetDeliveryNote(InputDeliverNoteDTO deliverIds)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var username = await _context.Users.Where(y => y.Id == userId).Select(y => y.Name).FirstOrDefaultAsync();

            return await _context.Journeyitems
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.Customer)
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.ReceipientCustomer)
                .Include(x => x.BookingItem).ThenInclude(x => x.BoxType)
                .Include(x => x.Journey)
                .Where(x => x.Id == deliverIds.JourneyItemId)
                .Select(x => new JourneyDeliveryNoteDTO
                {
                    ConsigneeName = x.BookingItem.Booking.ReceipientCustomer.Name,
                    ConsignorsName = x.BookingItem.Booking.Customer.Name,
                    BookingId = x.BookingItem.Booking.BookingId,
                    BoxType = x.BookingItem.BoxType.Name,
                    DeliveryIncharge = username,
                    TotalPackages = x.BookingItem.Quantity,
                    Amount = x.Quantity * x.BookingItem.UnitPrice,
                    CreatedDate = x.BookingItem.Booking.CreateDate,
                    Description = x.BookingItem.Description,
                    JourneyId = x.JourneyId,
                    JourneyItemId = x.Id,
                    NoOfPackages = x.Quantity,
                    JobNo = x.Journey.ContainerId,
                    DoNo = x.BookingItem.Booking.BookingId,
                    Mobile = x.BookingItem.Booking.ReceipientCustomer.Mobile,
                    Items = x.BookingItem.Description,
                    Rate = x.BookingItem.UnitPrice,
                    Remarks = x.Notes
                })
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<JourneyDeliveryNoteDTO>> GetAllDeliveryInvoiceNote(int JourneyId)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var username = await _context.Users.Where(y => y.Id == userId).Select(y => y.Name).FirstOrDefaultAsync();

            var journeyItemIds = await _context.Journeyitems                
                .Where(x=> x.JourneyId == JourneyId)
                .Select(x => x.Id)
                .ToArrayAsync();

            return await _context.Journeyitems
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.Customer)
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.ReceipientCustomer)
                .Include(x => x.BookingItem).ThenInclude(x => x.BoxType)
                .Include(x => x.Journey)
                .Where(x => journeyItemIds.Any(y => y == x.Id))
                .Where(x=> x.BookingItem.UnitPrice > 0)
                .Select(x => new JourneyDeliveryNoteDTO
                {
                    ConsigneeName = x.BookingItem.Booking.ReceipientCustomer.Name,
                    ConsignorsName = x.BookingItem.Booking.Customer.Name,
                    BookingId = x.BookingItem.Booking.BookingId,
                    BoxType = x.BookingItem.BoxType.Name,
                    DeliveryIncharge = username,
                    TotalPackages = x.BookingItem.Quantity,
                    Amount = x.Quantity * x.BookingItem.UnitPrice,
                    CreatedDate = x.BookingItem.Booking.CreateDate,
                    Description = x.BookingItem.Description,
                    JourneyId = x.JourneyId,
                    JourneyItemId = x.Id,
                    NoOfPackages = x.Quantity,
                    JobNo = x.Journey.ContainerId,
                    DoNo = x.BookingItem.Booking.BookingId,
                    Mobile = x.BookingItem.Booking.ReceipientCustomer.Mobile,
                    Items = x.BookingItem.Description,
                    Rate = x.BookingItem.UnitPrice,
                    Remarks = x.Notes
                }).ToListAsync();
        }
         public async Task<IEnumerable<JourneyDeliveryNoteDTO>> GetAllDeliveryNote(int JourneyId)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var username = await _context.Users.Where(y => y.Id == userId).Select(y => y.Name).FirstOrDefaultAsync();

            var journeyItemIds = await _context.Journeyitems                
                .Where(x=> x.JourneyId == JourneyId)
                .Select(x => x.Id)
                .ToArrayAsync();

            return await _context.Journeyitems
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.Customer)
                .Include(x => x.BookingItem).ThenInclude(x => x.Booking).ThenInclude(x => x.ReceipientCustomer)
                .Include(x => x.BookingItem).ThenInclude(x => x.BoxType)
                .Include(x => x.Journey)
                .Where(x => journeyItemIds.Any(y => y == x.Id))
                .Select(x => new JourneyDeliveryNoteDTO
                {
                    ConsigneeName = x.BookingItem.Booking.ReceipientCustomer.Name,
                    ConsignorsName = x.BookingItem.Booking.Customer.Name,
                    BookingId = x.BookingItem.Booking.BookingId,
                    BoxType = x.BookingItem.BoxType.Name,
                    DeliveryIncharge = username,
                    TotalPackages = x.BookingItem.Quantity,
                    Amount = x.Quantity * x.BookingItem.UnitPrice,
                    CreatedDate = x.BookingItem.Booking.CreateDate,
                    Description = x.BookingItem.Description,
                    JourneyId = x.JourneyId,
                    JourneyItemId = x.Id,
                    NoOfPackages = x.Quantity,
                    JobNo = x.Journey.ContainerId,
                    DoNo = x.BookingItem.Booking.BookingId,
                    Mobile = x.BookingItem.Booking.ReceipientCustomer.Mobile,
                    Items = x.BookingItem.Description,
                    Rate = x.BookingItem.UnitPrice,
                    Remarks = x.Notes
                }).ToListAsync();
        }


        public async Task<IEnumerable<JourneyLocationResponseDTO>> GetMyJourneyDeliveryLocations(int journeyId)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var list = await _context.Journeyitems
                .Include(x => x.DestinationHub)
                .Include(y => y.BookingItem.Booking).ThenInclude(address => address.ReceipientCustomerAddress).ThenInclude(loc => loc.Location)
                .Where(x => x.JourneyId == journeyId && x.Status != JourneyShipmentStatus.Delivered && x.Action == JourneyShipmentAction.Delivery)
                .ToListAsync();
            List<JourneyLocationResponseDTO> hubLocations = list
                 .Where(x => x.DestinationHubId != null)
                 .GroupBy(y => y.DestinationHub)
                 .Select(g => new JourneyLocationResponseDTO
                 {
                     Id = g.Key.Id,
                     Name = g.Key.Name,
                     Type = "H",
                     Address = g.Key.Address,
                     Count = g.Select(l => l).Distinct().Count()
                 }).ToList();

            List<JourneyLocationResponseDTO> CustomerLocations = list
               .Where(x => x.DestinationHubId == null)
               .GroupBy(y => y.BookingItem.Booking.ReceipientCustomerAddress.Location)
               .Select(g => new JourneyLocationResponseDTO
               {
                   Id = g.Key.Id,
                   Name = g.Key.Name,
                   Type = "C",
                   Address = g.Key.Pincode,
                   Count = g.Select(l => l).Distinct().Count()
               }).ToList();
            return hubLocations.Union(CustomerLocations).OrderByDescending(x => x.Type).OrderBy(y => y.Name);
        }
        public async Task<IEnumerable<JourneyLocationResponseDTO>> GetMyJourneyPickupLocations(int journeyId)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var list = await _context.Journeyitems
                .Include(x => x.OriginHub)
                .Include(y => y.BookingItem.Booking).ThenInclude(address => address.CustomerAddress).ThenInclude(loc => loc.Location)
                .Where(x => x.JourneyId == journeyId && x.Action == JourneyShipmentAction.Pickup)
                .ToListAsync();
            List<JourneyLocationResponseDTO> hubLocations = list
                 .Where(x => x.DestinationHubId != null)
                 .GroupBy(y => y.OriginHub)
                 .Select(g => new JourneyLocationResponseDTO
                 {
                     Id = g.Key.Id,
                     Name = g.Key.Name,
                     Type = "H",
                     Address = g.Key.Address,
                     Count = g.Select(l => l).Distinct().Count()
                 }).ToList();

            List<JourneyLocationResponseDTO> CustomerLocations = list
               .Where(x => x.DestinationHubId == null)
               .GroupBy(y => y.BookingItem.Booking.CustomerAddress.Location)
               .Select(g => new JourneyLocationResponseDTO
               {
                   Id = g.Key.Id,
                   Name = g.Key.Name,
                   Type = "C",
                   Address = g.Key.Pincode,
                   Count = g.Select(l => l).Distinct().Count()
               }).ToList();
            return hubLocations.Union(CustomerLocations).OrderByDescending(x => x.Type).OrderBy(y => y.Name);
        }

        public void updateBookingStatusDetails(int[] bookingItemIds)
        {
            var bookinglist = _context.Bookingitems.Include(x => x.Booking)
            .Where(x => bookingItemIds.Any(y => y == x.Id)).ToList();

            var bookingId = bookinglist.Select(x => x.BookingId).Distinct().ToArray();
            if (bookingId != null)
            {
                bookingId.ToList().ForEach(x =>
                {
                    var _bookingItemDetails = _context.Bookingitems.Where(y => y.BookingId == x).ToList();
                    var update = _context.Bookings.Where(r => r.Id == x).SingleOrDefault();
                    if (_bookingItemDetails.Sum(z => z.Quantity) == _bookingItemDetails.Sum(z => z.DeliveredQty))
                    {
                        update.IsClosed = true;
                        update.StatusId = BookingStatus.DeliveredShipment;
                    }
                    else
                    {
                        update.IsClosed = false;
                        update.StatusId = BookingStatus.PartialDelivered;
                    }
                });
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
