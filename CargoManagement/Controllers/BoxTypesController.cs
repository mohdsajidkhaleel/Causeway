using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxTypesController : ControllerBase
    {
        private readonly IBoxTypesService _service;

        public BoxTypesController(IBoxTypesService service)
        {
            _service = service;
        }

        [HttpGet("GetListOnOrderByName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetListOnOrderByName()
        {
            var res = await _service.GetListOnOrderByName();
            return new CMSResponse().Ok(res);
        }

    }
}
