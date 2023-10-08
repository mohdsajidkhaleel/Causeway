using CargoManagement.Models.Journey;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CargoManagement.Models.Shared;
using CargoManagement.Services.Extensions;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JourneyController : ControllerBase
    {
        private readonly IJourneyService _service;

        public JourneyController(IJourneyService service)
        {
            _service = service;
        }

        [HttpPost("GetJourneyList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Get(JourneyFilterDTO filter, [FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            var res = _service.Get(filter).PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Create(JourneyCreationDTO entry)
        {
            try
            {
                var res = await _service.Create(entry);
                return new CMSResponse().Created(res);
            }
            catch (Exception ex)
            {
                return new CMSResponse().UpdateFailed(ex.ToString());
            }            
        }

        [HttpPut("UpdateJourney")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Update(JourneyCreationDTO entry)
        {
            var res = await _service.Update(entry);
            if (res == 1)
            {
                return new CMSResponse().Updated(res);
            }
            else if (res == 2)
            {
                return new CMSResponse().UpdateFailed("Could't Update, Journey already Started");
            }
            else
                return new CMSResponse().UpdateFailed("Some error Occured");
        }

        [HttpDelete("DeleteJourney")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int journeyId)
        {
            try
            {
                var res = await _service.Delete(journeyId);
                if (res == 1)
                {
                    return new CMSResponse().Ok("Journey deleted");
                }
                else if (res == 2)
                {
                    return new CMSResponse().Ok("Could't Delete , Journey in Progress");
                }
                else
                {
                    return new CMSResponse().DeleteFailed("Journey not found");
                }

            }
            catch (Exception ex)
            {
                return new CMSResponse().DeleteFailed(ex.ToString());
            }
          
        }

        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetList()
        {
            var res = await _service.GetList();
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

        [HttpGet("PaginatedListScheduled")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> PaginatedListScheduled([FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            var res = _service.GetScheduledDetailsAsQueryable().PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("{JourneyId}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetById([FromRoute]int JourneyId)
        {
            var res = await _service.GetById(JourneyId);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("startJourney")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> StartJourney([FromQuery] int journeyId)
        {
            var res = await _service.StartJourney(journeyId);
            if (res)
            {
                return new CMSResponse().Ok(res);
            }
            else
            {
                return new CMSResponse().NotFound("Journey not found");
            }
        }

        [HttpPost("cancelJourney/{journeyId}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> CancelJourney([FromRoute] int journeyId)
        {
            var res = await _service.CancelJourney(journeyId);
            if (res)
            {
                return new CMSResponse().Ok(res);
            }
            else
            {
                return new CMSResponse().NotFound("Journey not found");
            }
        }

        [HttpPost("endJourney")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> EndJourney([FromQuery] int journeyId)
        {
            var res = await _service.EndJourney(journeyId);
            if (res)
            {
                return new CMSResponse().Ok(res);
            }
            else
            {
                return new CMSResponse().NotFound("Journey not found");
            }
        }

        [HttpGet("GetMyJourneyList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetMyJourneyList()
        {
            var res = await _service.GetMyJourneyList();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetMyJourneyDeliveryLocations")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetMyJourneyDeliveryLocations(int journeyId)
        {
            var res = await _service.GetMyJourneyDeliveryLocations(journeyId);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetMyJourneyPickupLocations")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetMyJourneyPickupLocations(int journeyId)
        {
            var res = await _service.GetMyJourneyPickupLocations(journeyId);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("Expenses")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> CreateJourneyExpense(List<JourneyExpenseCreationDTO> expenses)
        {
            var res = await _service.CreateJourneyExpense(expenses);
            return new CMSResponse().Ok(res);
        }

        [HttpPut("UpdateExpenses")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> UpdateExpenses(List<JourneyExpenseCreationDTO> expenses)
        {
            var res = await _service.UpdateExpenses(expenses);
            return new CMSResponse().Ok(res);
        }

        [HttpDelete("DeleteExpenses")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> DeleteExpenses(int Id)
        {
            var res = await _service.DeleteExpenses(Id);
            return new CMSResponse().Ok(res);
        }


        [HttpGet("GetExpenseList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetExpenseList(int? journeyId, [FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            var res =  _service.GetExpenseList(journeyId).PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetJourneyStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetJourneyStatus()
        {
            var res = new List<JourneyStatusDTO>
            {
                new JourneyStatusDTO{ JourneyStatusCode = "S",JourneyStatusName = "Scheduled"},
                new JourneyStatusDTO{ JourneyStatusCode = "I",JourneyStatusName = "InTransit"},
                new JourneyStatusDTO{ JourneyStatusCode = "E",JourneyStatusName = "Ended"},
                new JourneyStatusDTO{ JourneyStatusCode = "C",JourneyStatusName = "Cancelled"},
                new JourneyStatusDTO{ JourneyStatusCode = "D",JourneyStatusName = "Delivered"},
                new JourneyStatusDTO{ JourneyStatusCode = "PD",JourneyStatusName = "Partially Delivered"},
            };
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetActionStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetJourneyActionStatus()
        {
            var res = new List<JourneyActionDTO>
            {
                new JourneyActionDTO{ JourneyActionCode = "D",JourneyActionName = "Delivery"},
                new JourneyActionDTO{ JourneyActionCode = "P",JourneyActionName = "Pickup"}
            };
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetJourneyItemStatuses")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetJourneyItemStatus()
        {
            var res = new List<JourneyShipmentStatusDTO>
            {
                new JourneyShipmentStatusDTO{ JourneyShipmentStatusCode = "D",JourneyShipmentStatusName = "Delivered"},
                new JourneyShipmentStatusDTO{ JourneyShipmentStatusCode = "P",JourneyShipmentStatusName = "Pickedup"},
                new JourneyShipmentStatusDTO{ JourneyShipmentStatusCode = "S",JourneyShipmentStatusName = "Scheduled"},
                new JourneyShipmentStatusDTO{ JourneyShipmentStatusCode = "R",JourneyShipmentStatusName = "Received"},
                new JourneyShipmentStatusDTO{ JourneyShipmentStatusCode = "I",JourneyShipmentStatusName = "InTransit"},
                new JourneyShipmentStatusDTO{ JourneyShipmentStatusCode = "C",JourneyShipmentStatusName = "Cancelled"},
            };
            return new CMSResponse().Ok(res);
        }

        [HttpPost("JourneyPayment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> JourneyPayment(JourneyPaymentDTO paymentList)
        {
            var res = await _service.JourneyPayment(paymentList);
            if (res)
            {
                return new CMSResponse().Created(res);
            }
            else
                return new CMSResponse().UpdateFailed("Please End the Journey before payment or Please add unit price..!!");
        }

        [HttpPost("GetDeliveryNote")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> GetDeliveryNote(InputDeliverNoteDTO data)
        {
            var res = await _service.GetDeliveryNote(data);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetAllDeliveryInvoiceNote")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> GetAllDeliveryInvoiceNote(int JourneyId)
        {
            var res = await _service.GetAllDeliveryInvoiceNote(JourneyId);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetAllDeliveryNotes")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> GetAllDeliveryNote(int JourneyId)
        {
            var res = await _service.GetAllDeliveryNote(JourneyId);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("UpdateUnitPrice")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> UpdateUnitPrice(JourneyUpdateUnitPriceDTO UnitPriceList)
        {
            var res = await _service.UpdateUnitPrice(UnitPriceList);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("GetTodaysJourneyStartDetails")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> GetTodaysJourneyStartDetails(ManifestReportFilterDTO filterdata)
        {
            var res = await _service.GetTodaysJourneyStartDetails(filterdata);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetJourneyDetailsUnitPrice")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> GetJourneyDetailsUnitPrice(int journeyId)
        {
            var res = await _service.GetJourneyDetailsUnitPrice(journeyId);
            return new CMSResponse().Ok(res);
        }

        [HttpPost("UpdatePayLaterBy")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> PayLater(PayLaterDetailsDTO PayLaterDetails)
        {
            try
            {
                var res = await _service.UpdatePayLaterBy(PayLaterDetails);
                if (res == 1)
                {
                    return new CMSResponse().Ok(res);
                }
                else if (res == 2)
                {
                    return new CMSResponse().UpdateFailed("Please Update Unit Price before delivering");
                }
                else if (res == 3)
                {
                    return new CMSResponse().UpdateFailed("Pay later option can only done by consignor or Consignee");
                }
                else
                {
                    return new CMSResponse().BadRequest("Not Updated Successfully");
                }
            }
            catch (Exception ex)
            {
                return new CMSResponse().BadRequest(ex.ToString());
            }
            
        }
    }
}
