using CargoManagement.Models.JourneyItem;
using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JourneyItemsController : ControllerBase
    {
        private readonly IJourneyItemsService _service;

        public JourneyItemsController(IJourneyItemsService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int journeyId)
        {
            var res = await _service.Get(journeyId);
            return new CMSResponse().Ok(res);
        }
        [HttpGet("GetJourneyDetailByID")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetJourneyDetailByID([FromQuery] int journeyItemId)
        {
            var res = await _service.GetJourneyDetailByID(journeyItemId);
            return new CMSResponse().Ok(res);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Create(List<JourneyItemsCreationDTO> entries)
        {
            var res = await _service.Create(entries);
            return new CMSResponse().Created(res);
        }

        [HttpDelete("JourneyItemDelete")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Delete(int journeyDetailsId)
        {
            var res = await _service.Delete(journeyDetailsId);
            if (res == 1)
            {
                return new CMSResponse().Ok("Journey deleted");
            }
            else if((res == 2))
            {
                return new CMSResponse().DeleteFailed("Failed, Journey already processed");
            }
            else
            {
                return new CMSResponse().NotFound("Journey not found");
            }
        }

        [HttpPost("deliverItems")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> DeliverItems([FromBody] int[] journeyItemsIds)
        {
            try
            {
                if (journeyItemsIds.Count() > 0)
                {
                    var res = await _service.DeliverItems(journeyItemsIds);
                    if (res == 1)
                    {
                        return new CMSResponse().Ok("Shipment(s) Delivered");
                    }
                    else if (res == 2)
                    {
                        return new CMSResponse().UnprocessableEntity(null, "Please complete payment of Topay item before delivering to customer");
                    }
                    else if (res == 3)
                    {
                        return new CMSResponse().UnprocessableEntity(null, "Please end the Journey before Deliver");
                    }
                    else
                    {
                        return new CMSResponse().BadRequest("Some error occured while delivering item");
                    }
                }
                else
                {
                    return new CMSResponse().BadRequest("Atleast once item should be selected to deliver");
                }
            }
            catch (Exception ex)
            {
                return new CMSResponse().BadRequest(ex.ToString());
            }
            
        }

        [HttpPost("pickupItems")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> PickupItems([FromBody] int[] journeyItemsIds)
        {
            var res = await _service.PickupItems(journeyItemsIds);
            if (res)
            {
                return new CMSResponse().Ok("Items picked up");
            }
            else
            {
                return new CMSResponse().NotFound("Items not found");
            }
        }

        [HttpGet("GetBookingsForPickup")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingsForPickup(int journeyId, int locationID, bool locationIsHub)
        {
            var res = await _service.GetBookingsForPickup(journeyId, locationID, locationIsHub);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetBookingsForDelivery")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingsForDelivery(int journeyId, int locationID, bool locationIsHub)
        {
            var res = await _service.GetBookingsForDelivery(journeyId, locationID, locationIsHub);
            return new CMSResponse().Ok(res);
        }
    }
}
