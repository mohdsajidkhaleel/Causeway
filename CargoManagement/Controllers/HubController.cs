using CargoManagement.Models.Hubs;
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

    public class HubController : ControllerBase
    {
        private readonly IHubSerice _hubSerice;

        public HubController(IHubSerice hubSerice)
        {
            _hubSerice = hubSerice;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Get()
        {
            var res = await _hubSerice.Get();
            return new CMSResponse().Ok(res);
        }

        [HttpGet("PaginatedList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetDetailsAsQueryable([FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10)
        {
            var res = _hubSerice.GetDetailsAsQueryable().PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Create(HubCreationDTO hub)
        {
            var res = await _hubSerice.Create(hub);
            return new CMSResponse().Created(res);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int hubId)
        {
            var res = await _hubSerice.Delete(hubId);
            if (res)
            {
                return new CMSResponse().Ok("Hub deleted");
            }
            else
            {
                return new CMSResponse().NotFound("Hub not found");
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Update(HubUpdationDTO hub)
        {
            var updateHub = await _hubSerice.Update(hub);
            if (updateHub != null)
            {
                return new CMSResponse().Ok(updateHub);
            }
            else
            {
                return new CMSResponse().NotFound("Hub not found");
            }
        }
    }
}
