using CargoManagement.Models.Booking;
using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;
        private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;

        public BookingController(IBookingService service, IWhatsAppBusinessClient whatsAppBusinessClient)
        {
            _service = service;
            _whatsAppBusinessClient = whatsAppBusinessClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Get()
        {
            var res = await _service.Get();
            return new CMSResponse().Ok(res);
        }

        [HttpPost("list")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetList(BookingFilterDTO FilterList, [FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
           
        {          
            var res = _service.GetList(FilterList).PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("todaysBookingExport")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> TodaysBookingExport(BookingFilterDTO FilterList)

        {
            var res = await _service.TodaysBookingExport(FilterList);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("PaginatedList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> PaginatedList([FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            var res = _service.GetDetailsAsQueryable().PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetBookingCode")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingCode()
        {
            var res = await _service.GetBookingCode();
            return new CMSResponse().Ok(res);
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Create(BookingCreationDTO entry)
        {
            if (entry.BookingItems.Count() > 1)
            {
                return new CMSResponse().UpdateFailed("Some error occured in Booking; Please create fresh booking");
            }
            var res = await _service.Create(entry);
            return new CMSResponse().Created(res);
        }

        [HttpPut("UpdateBooking")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Update(BookingCreationDTO entry)
        {
            if (entry.BookingItems.Count() > 1)
            {
                return new CMSResponse().UpdateFailed("Some error occured in Booking; Please create fresh booking");
            }
            var res = await _service.Update(entry);
            if (res != null)
            {
                return new CMSResponse().Updated(res);
            }
            return new CMSResponse().UpdateFailed("Could'nt Update - Journey already created against this booking");

        }

        [HttpDelete("DeleteBooking")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int bookingId)
        {
            var res = await _service.Delete(bookingId);
            if (res == 1)
            {
                return new CMSResponse().Ok("Booking deleted");
            }
            else if (res == 2)
            {
                return new CMSResponse().DeleteFailed("Could't Delete , Journey already processed against this booking");
            }
            else
            {
                return new CMSResponse().NotFound("Booking not found");
            }
        }

        [HttpDelete("DeleteBookingItem")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> DeleteBookingItem([FromQuery] int bookingItemId)
        {
            var res = await _service.DeleteBookingItem(bookingItemId);
            if (res == 1)
            {
                return new CMSResponse().Ok("Booking Item deleted");
            }
            else if (res == 2)
            {
                return new CMSResponse().DeleteFailed("Could't Delete , Journey already processed against this booking");
            }
            else
            {
                return new CMSResponse().NotFound("Booking Item not found");
            }
        }



        [HttpPost("ReceivePayment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> ReceivePayment(int bookingId, int customerId, string paymentRemarks)
        {
            var res = await _service.ReceivePayment(bookingId, customerId, paymentRemarks);
            if (res == true)
            {
                return new CMSResponse().Ok(new { PaymentStatus = true, Message = "Payment Successful" });
            }
            else if (res == false)
            {
                return new CMSResponse().UnprocessableEntity(res, "Payment was already done. Unable to process");
            }
            else
            {
                return new CMSResponse().NotFound(res);
            }
        }

        [HttpGet("GetBookingDetails")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingDetails(int bookingId)
        {
            var res = await _service.GetBookingDetails(bookingId);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("StatusUpdate")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> StatusUpdate(BookingStatusUpdateDTO newStatus)
        {
            var res = await _service.UpdateBookingStatus(newStatus.BookingId, newStatus.JourneyId, newStatus.Status, newStatus.Comment, newStatus.FileName);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("Files")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingFiles(int bookingId)
        {
            var res = await _service.GetBookingFiles(bookingId);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("History")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingTransactions(int bookingId)
        {
            var res = await _service.GetBookingTransactions(bookingId);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetBookingStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingStatusCodes()
        {
            var res = await _service.GetBookingStatusCodes();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("ViewBookingStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> ViewBookingStatus(int BookingId)
        {
            var res = await _service.ViewBookingStatus(BookingId);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("ReturnBooking")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> ReturnBooking(int BookingId)
        {
            try
            {
                var res = await _service.ReturnBooking(BookingId);
                if (res == true)
                {
                    return new CMSResponse().Ok(new { ReturnStatus = true, Message = "Booking Returned Successful" });
                }
                else if (res == false)
                {
                    return new CMSResponse().UpdateFailed("This Booking shipment already progressed");
                }
                else
                {
                    return new CMSResponse().NotFound(res);
                }
            }
            catch (Exception ex)
            {
                return new CMSResponse().UpdateFailed("Error in booking multiple sequence");
            }
            
        }

        [HttpPost("SentWhatsappNotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> SendWhatsAppTextTemplateMessage(SendTemplateMessageViewModel Data)
        {            
            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(Data);
            if (results != null)
            {
                return new CMSResponse().Ok(new { ReturnStatus = true, Message = "Notification sent successfully" });
            }
            else
            {
                return new CMSResponse().Ok(new { ReturnStatus = false, Message = "Notification not sent" });
            }
        }
    }
}
