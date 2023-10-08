using CargoManagement.Models.CustomerAddress;
using CargoManagement.Models.Customers;
using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Get(CustomerFilterDTO filter, [FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            try
            {
                var result = _service.Get(filter).PaginatedResponse(PageNumber, PageSize);
                var finalResult = await _service.validateCustomerOutstandings(result.List); // just to validate the payment outstanding is equal

                var res =  new PaginatedResponse { List = finalResult.ToList(), TotalCount = result.TotalCount };
                return new CMSResponse().Ok(res);
            }
            catch (Exception ex)
            {
                return new CMSResponse().BadRequest(ex.ToString());
                throw;
            }
            
        }

        [HttpGet("GetDropdownCustomer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetDropdownCustomer()
        {
            var res = await _service.GetDropdownCustomer();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("PaginatedList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetDetailsAsQueryable([FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            var res = _service.GetDetailsAsQueryable().PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Create(CustomerCreationDTO entry)
        {
            var res = await _service.Create(entry);
            if (res != null)
            {
                return new CMSResponse().Created(res);
            }
            return new CMSResponse().UpdateFailed("Invalid Data");
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int id)
        {
            try
            {
                var res = await _service.Delete(id);
                if (res)
                {
                    return new CMSResponse().Ok("Customer deleted");
                }
                else
                {
                    return new CMSResponse().NotFound("Customer not found");
                }
            }
            catch (Exception ex)
            {
                return new CMSResponse().DeleteFailed("Booking already Placed against this Customer");
            }
            
        }

        [HttpPut("CustomerUpdate")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Update(CustomerUpdationDTO entry)
        {
            var updated = await _service.Update(entry);
            if (updated != null)
            {
                return new CMSResponse().Ok("Customer updated");
            }
            else
            {
                return new CMSResponse().NotFound("Customer not found");
            }
        }

        [HttpGet("GetCustomerByMobile")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetCustomerByMobile([FromQuery] string mobileNumber)
        {
            var customer = await _service.GetCustomerByMobile(mobileNumber);
            if (customer != null)
            {
                return new CMSResponse().Ok(customer);
            }
            else
            {
                return new CMSResponse().NotFound(null);
            }
        }

        [HttpGet("GetCustomerById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetCustomerById([FromQuery] int Id)
        {
            var customer = await _service.GetCustomerById(Id);
            if (customer != null)
            {
                return new CMSResponse().Ok(customer);
            }
            else
            {
                return new CMSResponse().NotFound(null);
            }
        }


        [HttpGet("SearchByMobileOrName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> SearchByMobileOrName([FromQuery] string mobileNumber)
        {
            var customer = await _service.GetCustomerLikeMobileOrName(mobileNumber);
            if (customer != null)
            {
                return new CMSResponse().Ok(customer);
            }
            else
            {
                return new CMSResponse().NotFound(null);
            }
        }

        [HttpPost("ReceivePayment/{customerId}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> ReceivePayment([FromRoute] int customerId, [FromQuery] decimal amount)
        {
            var res = await _service.ReceivePayment(customerId, amount);
            return new CMSResponse().Ok(res);
        }


        [HttpPut("UpdateCreditSetting")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> UpdateCreditSetting(CustomerUpdationDTO entry)
        {
            var res = await _service.UpdateCreditSetting(entry.Id,entry.IsCreditAllowed,(decimal)entry.CreditLimit);
            return new CMSResponse().Created(res);
        }


        #region Customer Transactions

        [HttpGet("TransactionsPaginatedList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> TransactionsPaginatedList([FromQuery] int customerId, [FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            var res = _service.GetCustomerTransactionAsQueryable(customerId).PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        #endregion


        #region Customer Address

        [HttpPost("CreateAddress")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> CreateAddress(CustomerAddressCreationDTO entry)
        {
            var res = await _service.CreateAddress(entry);
            return new CMSResponse().Created(res);
        }

        [HttpDelete("Address")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> DeleteAddress([FromQuery] int id)
        {
            var res = await _service.DeleteAddress(id);
            if (res)
            {
                return new CMSResponse().Ok("Customer address deleted");
            }
            else
            {
                return new CMSResponse().NotFound("Customer address not found");
            }
        }

        [HttpPut("UpdateAddress")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> UpdateAddress(CustomerAddressCreationDTO entry)
        {
            var updated = await _service.UpdateAddress(entry);
            if (updated != null)
            {
                return new CMSResponse().Ok("Customer address updated");
            }
            else
            {
                return new CMSResponse().NotFound("Customer address not found");
            }
        }

        [HttpGet("YetToPayBookingDetails")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetYetToPayBookingDetails(int CustomerId)
        {
            var res = await _service.YetToPayBookingDetails(CustomerId);
            return new CMSResponse().Created(res);
        }

        [HttpPost("PayOutstandingAmount")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> PayOutstandingAmount(CustomerMasterOutstandingPaymentDTO PayDetails)
        {
            if (PayDetails.PaymentList.Count() > 0)
            {
                var res = await _service.PayOutstandingAmount(PayDetails);
                return new CMSResponse().Created(res);
            }
            else
            {
                return new CMSResponse().BadRequest("Please select atleast one booking entry and Pay");
            }            
        }

        [HttpGet("GetPaymentHistoryByCustomerId")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetPaymentHistoryByCustomerId(int CustomerId)
        {
            var res = await _service.GetPaymentHistoryByCustomerId(CustomerId);
            return new CMSResponse().Created(res);
        }

        [HttpGet("GetMonthlyReport")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetMonthlyReport(DateTime date)
        {
            var res = await _service.GetMonthlyReport(date);
            return new CMSResponse().Created(res);
        }

        [HttpGet("GetDailyCollectionReport")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetDailyCollectionReport(DateTime date)
        {
            var res = await _service.GetDailyCollectionReport(date);
            return new CMSResponse().Created(res);
        }

        [HttpGet("GetJobCollectionReport")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetJobCollectionReport(int jobId)
        {
            var res = await _service.GetJobCollectionReport(jobId);
            return new CMSResponse().Created(res);
        }

        [HttpGet("GetTotalCustomerOutstandings")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetTotalCustomerOutstandings()
        {
            var res = await _service.GetTotalCustomerOutstandings();
            return new CMSResponse().Ok(res);
        }

        #endregion
    }
}
