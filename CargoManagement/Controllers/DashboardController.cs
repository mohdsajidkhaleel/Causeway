using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DashboardController : ControllerBase
    {

        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("TodaysNewBookings")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> TodaysNewBookings()
        {
            var res = await _service.TodaysNewBookings();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("TodaysClosedBookings")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> TodaysClosedBookings()
        {
            var res = await _service.TodaysClosedBookings();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("TodaysScheduledJourneys")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> TodaysScheduledJourneys()
        {
            var res = await _service.TodaysScheduledJourneys();
            return new CMSResponse().Ok(res);
        }


        [HttpGet("OutstandingBookings")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> OutstandingBookings()
        {
            var res = await _service.OutstandingBookings();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("OutstandingCustomerCredit")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> OutstandingCustomerCredit()
        {
            var res = _service.OutstandingCustomerCredit();
            return new CMSResponse().Ok(res);
        }

    }
}
