using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropDownController : ControllerBase
    {
        private readonly IDropdownService _service;

        public DropDownController(IDropdownService service)
        {
            _service = service;
        }

        [HttpGet("Boxtypes")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Get()
        {
            var res = await _service.Boxtypes();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("States")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> States()
        {
            var res = await _service.States();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("Districts")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Districts([FromQuery] int stateId)
        {
            var res = await _service.Districts(stateId);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("Locations")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Locations([FromQuery] int districtId)
        {
            var res = await _service.Locations(districtId);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("UserTypes")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> UserTypes([FromQuery] int districtId)
        {
            var res = await _service.UserTypes();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("HubTypes")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> HubTypes([FromQuery] int districtId)
        {
            var res = await _service.HubTypes();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("Status")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Status()
        {
            var res = await _service.Status();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("ExpenseTypes")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> ExpenseTypes()
        {
            var res = await _service.ExpenseTypes();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetBookingId")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetBookingId()
        {
            var res = await _service.GetBookingId();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetManifestJourneyDetails")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetManifestJourneyDetails(DateTime DateofDispatch)
        {
            var res = await _service.GetManifestJourneyDetails(DateofDispatch);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetJourneyName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetJourneyName()
        {
            var res = await _service.GetJourneyName();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("GetUserRoles")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetUserRoles()
        {
            var res = await _service.GetUserRoles();
            return new CMSResponse().Ok(res);
        }
    }
}
