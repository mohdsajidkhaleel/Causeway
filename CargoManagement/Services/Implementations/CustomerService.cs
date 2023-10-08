using AutoMapper;
using CargoManagement.Models.Customers;
using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using CargoManagement.Services.Extensions;
using CargoManagement.Models.CustomerTransactions;
using CargoManagement.Models.CustomerAddress;
using CargoManagement.Models.Shared;
using System.Collections.Generic;

namespace CargoManagement.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(cmspartialdeliveryContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CustomerResponseDTO> Create(CustomerCreationDTO customer)
        {
            var prevData = await _context.Customers
                .Where(x => x.Mobile == customer.Mobile)
                .SingleOrDefaultAsync();

            if (prevData == null)
            {
                Customer newEntry = _mapper.Map<Customer>(customer);
                newEntry.Mobile = customer.Mobile.Trim();
                newEntry.CreatedBy = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
                newEntry.HubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));//Only hub user can create a new user
                await _context.Customers.AddAsync(newEntry);
                await _context.SaveChangesAsync();
                return _mapper.Map<CustomerResponseDTO>(newEntry);
            }
            return null;
        }

        public async Task<bool> Delete(int custId)
        {
            var itemToRemove = _context.Customers.Include(x => x.Customeraddresses)
                .SingleOrDefault(x => x.Id == custId);
            if (itemToRemove != null)
            {
                _context.Remove<Customer>(itemToRemove);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public IQueryable<CustomerResponseDTO> Get(CustomerFilterDTO filter)
        {
            var fullList = _context.Customers
                .Include(cust => cust.Customeraddresses)
                .AsQueryable();

            if (filter.showCredit == 1)
            {
                fullList = fullList.Where(x => x.OutstandingCredit > 0).AsQueryable();
            }
            if (filter.Name != null)
            {
                fullList = fullList.Where(x => x.Name.Contains(filter.Name.Trim())).AsQueryable();
            }
            if (filter.Mobile != null)
            {
                fullList = fullList.Where(x => x.Mobile.Contains(filter.Mobile.Trim())).AsQueryable();
            }
            return _mapper.ProjectTo<CustomerResponseDTO>(fullList);
        }

        public async Task<IEnumerable<CustomerResponseDTO>> validateCustomerOutstandings(object customerList)
        {
            try
            {
                var objectListData = _mapper.Map<IEnumerable<CustomerResponseDTO>>(customerList);
                foreach (var obj in objectListData)
                {
                    var result = await YetToPayBookingDetails(obj.Id);
                    obj.OutstandingCredit = result.ToList().Sum(x => x.AmountToPay);
                }
                return objectListData;
            }
            catch (Exception ex)
            {

                throw;
            }            
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetTotalCustomerOutstandings()
        {
            var list = await _context.Customers
                .Include(cust => cust.Customeraddresses)
                .Where(x => x.OutstandingCredit > 0)
              .ToListAsync();
            return _mapper.Map<IEnumerable<CustomerResponseDTO>>(list);
        }

        public async Task<IEnumerable<DropdownCustomerResponseDTO>> GetDropdownCustomer()
        {
            var list = await _context.Customers
                  .Include(cust => cust.Customeraddresses)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DropdownCustomerResponseDTO>>(list);
        }

        public async Task<IEnumerable<CustomerBookingPaymentDTO>> YetToPayBookingDetails(int CustomerId)
        {
            return await _context.Bookingpayments
                .Include(x => x.Booking).ThenInclude(x => x.Bookingitems).ThenInclude(x => x.Journeyitems).ThenInclude(x => x.Journey)
                .Include(x => x.Booking).ThenInclude(x => x.Customer)
                .Include(x => x.Booking).ThenInclude(x => x.ReceipientCustomer)
                .Where(x => x.PayLaterBy == CustomerId)
                .Select(x => new CustomerBookingPaymentDTO
                {
                    PaymentId = x.Id,
                    BookingCode = x.Booking.BookingId,  //Booking Code
                    BookingId = x.BookingId,
                    BookingItemId = x.BookingItemId,
                    BookingDate = x.Booking.CreateDate,
                    BookingItemNote = x.BookingItem.Description,
                    ConsignorName = x.Booking.Customer.Name,
                    ConsigneeName = x.Booking.ReceipientCustomer.Name,
                    CustomerId = CustomerId,
                    Quantity = x.BookingItem.Quantity,
                    DispatchedQuantity = x.TotalDispatchedQuantity,
                    TotalAmount = x.BookingItem.Quantity * x.BookingItem.UnitPrice,
                    AmountToPay = x.TotalDispatchedQuantity * x.BookingItem.UnitPrice,
                    FromLocation = "Dubai",
                    ToLocation = "Qatar",
                    JourneyItemId = x.JourneyItemId,
                    JourneyId = x.JourneyId,
                    JourneyName = x.JourneyItemId != null ? x.BookingItem.Journeyitems
                         .Where(y => y.Id == x.JourneyItemId && y.JourneyId == x.JourneyId).SingleOrDefault().Journey.Name : ""
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentHistoryDTO>> GetPaymentHistoryByCustomerId(int CustomerId)
        {
            return await _context.Bookingpayments
                .Include(x => x.Booking).ThenInclude(x => x.Bookingitems).ThenInclude(x => x.Journeyitems).ThenInclude(x => x.Journey)
                .Include(x => x.Booking).ThenInclude(x => x.Customer)
                .Include(x => x.Booking).ThenInclude(x => x.ReceipientCustomer)
                .Where(x => x.Paidby == CustomerId)
                .Select(x => new PaymentHistoryDTO
                {
                    PaymentId = x.Id,
                    BookingCode = x.Booking.BookingId,  //Booking Code
                    BookingId = x.BookingId,
                    ConsignorName = x.Booking.Customer.Name,
                    ConsigneeName = x.Booking.ReceipientCustomer.Name,
                    TotalQuantity = x.BookingItem.Quantity,
                    TotalPaidQuantity = x.TotalDispatchedQuantity,
                    TotalAmountPaid = x.TotalAmountPaid,
                    Discount = x.Discount,
                    JourneyId = x.JourneyId,
                    ContainerName = x.Journey.ContainerId,
                    PaidDate = x.Paiddate,
                    JourneyName = x.JourneyItemId != null ? x.BookingItem.Journeyitems
                         .Where(y => y.Id == x.JourneyItemId && y.JourneyId == x.JourneyId).SingleOrDefault().Journey.Name : ""
                })
                .ToListAsync();
        }

        public async Task<MonthlyPLReportDetailDTO> GetMonthlyReport(DateTime date)
        {
            MonthlyPLReportDetailDTO finalData = new MonthlyPLReportDetailDTO();

            var Jobdata = await _context.Bookingpayments
                .Include(x => x.Journey)
                .Include(x => x.Booking)
                .Where(x => x.Paiddate.Value.Month == date.Month && x.Paiddate.Value.Year == date.Year)
                .OrderByDescending(x => x.BookingId)
                .ToListAsync();
            //&& x.JourneyId != null && x.TotalAmountPaid > 0)

            var Expensedata = await _context.Journeyexpenses
                .Include(x => x.ExpenseType)
                .Include(x=> x.Journey)
                .Where(x => x.CreateDate.Value.Month == date.Month && x.CreateDate.Value.Year == date.Year)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            if (Expensedata.Count() > 0)
            {
                var expData = Expensedata.Where(x => x.JourneyId == null)
                    .GroupBy(x => x.ExpenseType)
                    .Select(x => new MonthlyOfficeExpenseReportDetailDTO
                    {
                        ExpenseAmount = x.Sum(x => x.Amount),
                        ExpenseName = x.Key.Name
                    });
                finalData.OfficeExpenses.AddRange(expData);
            
                 var jobexpData = Expensedata.Where(x => x.JourneyId != null)
                    .GroupBy(x => x.Journey)
                    .Select(x => new MonthlyJobExpenseReportDetailDTO
                    {
                        JobExpenseAmount = x.Sum(x => x.Amount),
                        JobName = "Total Job Expense against " + x.Key.Name
                    });
                finalData.JobExpense.AddRange(jobexpData);
            }

            if (Jobdata.Count() > 0)
            {
                var Amount_with_JourneyId = Jobdata.FindAll(x => x.Journey != null).ToList(); // In paymentTable , Journey is created
                var Amount_withOut_JourneyId = Jobdata.FindAll(x => x.Journey == null).ToList(); // In paymentTable , Journey is not created

                if (Amount_with_JourneyId.Count() > 0)
                {
                    var jobexpense = Amount_with_JourneyId
                    .GroupBy(x => x.Journey)
                    .Select(x => new MonthlyJourneyReportDetailDTO
                    {
                        ReceivedAmount = x.Sum(x => x.TotalAmountPaid),
                        JobNo = x.Key.Name
                    });
                    finalData.JourneyAmountReceived.AddRange(jobexpense);
                }                

                if (Amount_withOut_JourneyId.Count() > 0)
                {
                    var jobexpense_withoutJourney = Amount_withOut_JourneyId
                   .Select(x => new MonthlyJourneyReportDetailDTO
                   {
                       ReceivedAmount = x.TotalAmountPaid,
                       JobNo = "Advance Amount against BookingId :~ " + x.Booking.BookingId
                   });
                    finalData.JourneyAmountReceived.AddRange(jobexpense_withoutJourney);
                }             

                //var uniqueJourneyIds = Jobdata.DistinctBy(x => x.JourneyId).ToList();
                //uniqueJourneyIds.ForEach(dd =>
                //{
                //    MonthlyJourneyReportDetailDTO customData = new MonthlyJourneyReportDetailDTO
                //    {
                //        ReceivedAmount = Jobdata.Where(y => y.JourneyId == dd.JourneyId).Sum(y => y.TotalAmountPaid),
                //        JobNo = Jobdata.Where(x => x.JourneyId == dd.JourneyId).FirstOrDefault().Journey.Name
                //    };
                //    finalData.JourneyAmountReceived.Add(customData);
                //});

                return finalData;
            }
            else
                return finalData;
        }

        public async Task<IEnumerable<DailyCollectionReportDetailDTO>> GetDailyCollectionReport(DateTime date)
        {
            return await _context.Bookingpayments
               .Include(x => x.BookingItem).ThenInclude(x => x.Journeyitems).ThenInclude(x => x.Journey)
               .Include(x => x.Booking)
               .Where(x => (x.Paiddate.Value.Day == date.Day && x.Paiddate.Value.Month == date.Month && x.Paiddate.Value.Year == date.Year)
               && x.TotalAmountPaid > 0)
               .Select(x => new DailyCollectionReportDetailDTO
               {
                   BookingNumber = x.Booking.BookingId,
                   JobNumber = x.Journey != null ? x.Journey.Name : "",
                   ReceivedAmount = x.TotalAmountPaid,
                   ReceivedDate = x.Paiddate
               })
               .OrderByDescending(x => x.BookingNumber)
               .ToListAsync();
        }

        public async Task<JobCollectionReportDetailDTO> GetJobCollectionReport(int jobId)
        {
            JobCollectionReportDetailDTO finalData = new JobCollectionReportDetailDTO();

            var Jobdata = await _context.Bookingpayments
                .Include(x => x.BookingItem).ThenInclude(x => x.Journeyitems).ThenInclude(x => x.Journey)
                .Include(x => x.Booking)
                .Where(x => x.JourneyId == jobId && x.TotalAmountPaid > 0)
                .ToListAsync();
            var Expensedata = await _context.Journeyexpenses
                .Include(x => x.ExpenseType)
                .Where(x => x.JourneyId == jobId)
                .ToListAsync();

            if (Jobdata.Count() > 0)
            {
                var Data = Jobdata
                       .Select(x => new JobReportDetailDTO
                       {
                           BookingNumber = x.Booking.BookingId,
                           Date = x.Paiddate,
                           ReceivedAmount = x.TotalAmountPaid
                       })
                       .OrderByDescending(x => x.Date);
                finalData.JourneyCollection.AddRange(Data);

                if (Expensedata.Count() > 0)
                {
                    var expData = Expensedata
                        .Select(x => new JobExpenseReportDetailDTO
                        {
                            ExpenseAmount = x.Amount,
                            ExpenseName = x.ExpenseType.Name,
                            Date = x.CreateDate
                        })
                        .OrderByDescending(x => x.Date);
                    finalData.JobExpenses.AddRange(expData);
                }
                return finalData;
            }
            else
                return null;
        }

        public IQueryable<CustomerDetailsDTO> GetDetailsAsQueryable()
        {
            var hubId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("HubId"));
            var list = _context.Customers
                .Include(cust => cust.Customeraddresses)
              .AsQueryable();
            Console.WriteLine(list.ToQueryString());
            return _mapper.ProjectTo<CustomerDetailsDTO>(list);
        }

        public IQueryable<CustomerTransactionsDTO> GetCustomerTransactionAsQueryable(int customerId)
        {
            var list = _context.Customertransactions
              .Where(cust => cust.CustomerId == customerId)
              .OrderByDescending(cust => cust.CreateDate)
              .AsQueryable();
            Console.WriteLine(list.ToQueryString());
            return _mapper.ProjectTo<CustomerTransactionsDTO>(list);
        }

        public async Task<CustomerResponseDTO> Update(CustomerUpdationDTO customer)
        {
            //var prevData = await _context.Customers
            //    .Where(x => x.Mobile == customer.Mobile)
            //    .SingleOrDefaultAsync();

            //if (prevData == null)
            //{
            //    var itemToUpdate = _context.Customers.SingleOrDefault(x => x.Id == customer.Id);
            //    if (itemToUpdate != null)
            //    {
            //        itemToUpdate.Name = customer.Name;
            //        itemToUpdate.Email = customer.Email;
            //        itemToUpdate.Mobile = customer.Mobile;
            //        itemToUpdate.IsCreditAllowed = customer.IsCreditAllowed;
            //        itemToUpdate.CreditLimit = customer.IsCreditAllowed ? customer.CreditLimit : 0;
            //        itemToUpdate.UpdatedBy = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            //        _context.Update(itemToUpdate);
            //        await _context.SaveChangesAsync();
            //        return _mapper.Map<CustomerResponseDTO>(itemToUpdate);
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //else
            //{
            //    return null;
            //}

            var itemToUpdate = await _context.Customers
                .Where(x => x.Id == customer.Id)
                .SingleOrDefaultAsync();

            if (itemToUpdate != null)
            {
                var _existingMobileNumber = await _context.Customers.Where(x => x.Mobile == customer.Mobile.Trim()).ToListAsync();

                if (_existingMobileNumber.Count() > 1)
                {
                    return null;
                }

                itemToUpdate.Name = customer.Name;
                itemToUpdate.Email = customer.Email;
                itemToUpdate.Mobile = customer.Mobile;
                itemToUpdate.IsCreditAllowed = customer.IsCreditAllowed;
                itemToUpdate.CreditLimit = customer.IsCreditAllowed ? customer.CreditLimit : 0;
                itemToUpdate.UpdatedBy = Convert.ToInt32(_httpContextAccessor?.HttpContext?.GetClaim("UserId"));
                _context.Update(itemToUpdate);
                await _context.SaveChangesAsync();
                return _mapper.Map<CustomerResponseDTO>(itemToUpdate);
            }
            else
            {
                return null;
            }

        }

        public async Task<CustomerDetailsDTO> GetCustomerByMobile(string mobileNumber)
        {
            var customer = await _context.Customers
                .Include(cust => cust.Customeraddresses).ThenInclude(custAdd => custAdd.Location).ThenInclude(loc => loc.District).ThenInclude(dist => dist.State)
                .FirstOrDefaultAsync(cust => cust.Mobile == mobileNumber || cust.Customeraddresses.Any(x => x.Mobile == mobileNumber));
            return _mapper.Map<CustomerDetailsDTO>(customer);
        }
        public async Task<CustomerDetailsDTO> GetCustomerById(int Id)
        {
            var customer = await _context.Customers
                .Include(cust => cust.Customeraddresses).ThenInclude(custAdd => custAdd.Location).ThenInclude(loc => loc.District).ThenInclude(dist => dist.State)
                .FirstOrDefaultAsync(cust => cust.Id == Id);
            return _mapper.Map<CustomerDetailsDTO>(customer);
        }   
        public async Task<List<CustomerDetailsDTO>> GetCustomerLikeMobileOrName(string searchtext)
        {
            var customers = await _context.Customers
                .Include(cust => cust.Customeraddresses).ThenInclude(custAdd => custAdd.Location).ThenInclude(loc => loc.District).ThenInclude(dist => dist.State)
                .Where(cust => cust.Mobile.Contains(searchtext) || cust.Name.Contains(searchtext) || cust.Customeraddresses.Any(x => x.Mobile.Contains(searchtext))).ToListAsync();
            return _mapper.Map<List<CustomerDetailsDTO>>(customers);
        }

        public async Task<bool> ReceivePayment(int customerId, decimal amount)
        {
            Customer cust = _context.Customers.FirstOrDefault(cust => cust.Id == customerId);
            var currentOutStanding = cust.OutstandingCredit;
            var newAmount = currentOutStanding - amount;

            cust.OutstandingCredit = newAmount;
            cust.Customertransactions.Add(new Customertransaction() { CustomerId = customerId, PreviousAmount = (decimal)currentOutStanding, TransactionAmount = amount, NewAmount = (decimal)newAmount, Description = "Customer Payment" });
            _context.Update(cust);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCreditSetting(int customerId, bool isCredit, decimal creditLimit)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));

            var entity = new Customer() { Id = customerId, IsCreditAllowed = isCredit, CreditLimit = creditLimit, UpdatedBy = userId };
            var attachedEntity = _context.Customers.Attach(entity);
            attachedEntity.Property(x => x.IsCreditAllowed).IsModified = true;
            attachedEntity.Property(x => x.CreditLimit).IsModified = true;
            attachedEntity.Property(x => x.UpdatedBy).IsModified = true;
            await _context.SaveChangesAsync();
            return true;
        }

        #region public for internal calls

        public void UpdateCustomerCredit(int customerId, decimal amount, string bookingCode)
        {
            Customer cust = _context.Customers.FirstOrDefault(cust => cust.Id == customerId);
            var currentOutStanding = cust.OutstandingCredit;
            var newAmount = currentOutStanding + amount;

            cust.OutstandingCredit = newAmount;
            cust.Customertransactions.Add(new Customertransaction() { CustomerId = customerId, PreviousAmount = (decimal)currentOutStanding, TransactionAmount = amount, NewAmount = (decimal)newAmount, Description = "From Booking " + bookingCode });
            _context.Update(cust);
        }
        #endregion


        #region For Customer address


        public async Task<CustomerAdderssResponseDTO> CreateAddress(CustomerAddressCreationDTO customerAddress)
        {
            Customeraddress newEntry = _mapper.Map<Customeraddress>(customerAddress);
            newEntry.CreatedBy = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            await _context.Customeraddresses.AddAsync(newEntry);
            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerAdderssResponseDTO>(newEntry);
        }

        public async Task<CustomerAdderssResponseDTO> UpdateAddress(CustomerAddressCreationDTO customerAddress)
        {
            var itemToUpdate = _context.Customeraddresses.SingleOrDefault(x => x.Id == customerAddress.Id);
            if (itemToUpdate != null)
            {
                itemToUpdate.Address = customerAddress.Address;
                itemToUpdate.StateId = customerAddress.StateId;
                itemToUpdate.DistrictId = customerAddress.DistrictId;
                itemToUpdate.LocationId = customerAddress.LocationId;
                itemToUpdate.Pincode = customerAddress.Pincode;
                itemToUpdate.Landmark = customerAddress.Landmark;
                itemToUpdate.Description = customerAddress.Description;
                itemToUpdate.Mobile = customerAddress.Mobile;
                itemToUpdate.UpdatedBy = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
                _context.Update(itemToUpdate);
                await _context.SaveChangesAsync();
                return _mapper.Map<CustomerAdderssResponseDTO>(itemToUpdate);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteAddress(int custAddressId)
        {
            var itemToRemove = _context.Customeraddresses
                .SingleOrDefault(x => x.Id == custAddressId);
            if (itemToRemove != null)
            {
                _context.Remove<Customeraddress>(itemToRemove);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> PayOutstandingAmount(CustomerMasterOutstandingPaymentDTO PayDetails)
        {
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.GetClaim("UserId"));
            var _totalAmountSum = 0;
            decimal EachDiscountAmount = 0;
            if (PayDetails.Discount > 0)
            {
                EachDiscountAmount = PayDetails.Discount / PayDetails.PaymentList.Count();
            }
            var paymentIDarray = PayDetails.PaymentList.Select(x => x.PaymentId).ToArray();
            var bookingIDarray = PayDetails.PaymentList.Select(x => x.BookingId).Distinct().ToArray();
            var _bookingPaymentList = await _context.Bookingpayments
                .Include(x => x.Booking).ThenInclude(x => x.Bookingitems)
                .Where(x => paymentIDarray.Any(y => y == x.Id))
                .ToListAsync();
            var customer = await _context.Customers
                .Where(x => x.Id == PayDetails.PaymentList[0].CustomerId)
                .SingleOrDefaultAsync();

            if (_bookingPaymentList != null && customer != null)
            {
                foreach (var objPayment in _bookingPaymentList)
                {
                    objPayment.IsPaymentCompleted = true;
                    objPayment.TotalAmountPaid = (objPayment.TotalDispatchedQuantity * objPayment.BookingItem.UnitPrice) - EachDiscountAmount;
                    _totalAmountSum = _totalAmountSum + Convert.ToInt32(objPayment.TotalDispatchedQuantity * objPayment.BookingItem.UnitPrice);
                    objPayment.PayLaterBy = null;
                    objPayment.Paidby = PayDetails.PaymentList[0].CustomerId;
                    objPayment.Paiddate = DateTime.UtcNow;
                    objPayment.Discount = EachDiscountAmount;

                }
                customer.OutstandingCredit = customer.OutstandingCredit - _totalAmountSum;
                await _context.SaveChangesAsync();

                var _bookinglist = await _context.Bookings
                    .Include(x => x.Bookingpayments).ThenInclude(x => x.BookingItem)
                    .Include(x => x.Bookingitems)
                    .Where(x => bookingIDarray.Any(y => y == x.Id))
                    .ToListAsync();

                foreach (var _objBooking in _bookinglist)
                {
                    if (_objBooking.PaymentMode.Equals(PaymentStatus.ToPay))
                    {
                        
                        if (_objBooking.Bookingpayments.Sum(x => x.TotalDispatchedQuantity) == _objBooking.Bookingitems.Select(x => x.Quantity).FirstOrDefault())
                        {
                            _objBooking.PaidBy = PayDetails.PaymentList[0].CustomerId;
                            _objBooking.PayLaterBy = null;
                            _objBooking.PaidDate = DateTime.UtcNow;
                        }
                        _objBooking.TotalAmountReceived = _objBooking.TotalAmountReceived + (_objBooking.Bookingitems.Select(x => x.Quantity * x.UnitPrice).FirstOrDefault() - EachDiscountAmount);
                        _objBooking.TotalDiscountGiven = _objBooking.TotalDiscountGiven + EachDiscountAmount;
                    }
                }
                if (PayDetails.Discount > 0)
                {
                    Customerdiscount _discount = new Customerdiscount
                    {
                        CreatedDate = DateTime.UtcNow,
                        CustomerId = PayDetails.CustomerId,
                        DiscountAmount = PayDetails.Discount,
                        DiscountGivenBy = userId,
                        DiscountBookingIds = String.Join(",", PayDetails.PaymentList.Select(x => Convert.ToString(x.BookingId)))
                    };
                    await _context.Customerdiscounts.AddAsync(_discount);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion
    }
}
