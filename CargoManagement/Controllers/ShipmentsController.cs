using CargoManagement.Models.Booking;
using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _service;

        public ShipmentsController(IShipmentService service)
        {
            _service = service;
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

        [HttpGet("GetDeliverables")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetDeliverables()
        {
            var res = await _service.GetDeliverables();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetPickups")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetPickups()
        {
            var res = await _service.GetPickups();
            return new CMSResponse().Ok(res);
        }


        [HttpGet("GetDeliverablesByHub")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetDeliverablesByHub(int HubId)
        {
            var res = await _service.GetDeliverablesByHub(HubId);
            return new CMSResponse().Ok(res);
        }
    }
}
