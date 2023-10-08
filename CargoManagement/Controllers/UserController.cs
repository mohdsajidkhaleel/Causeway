using CargoManagement.Models.Shared;
using CargoManagement.Models.User;
using CargoManagement.Services.Abstractions;
using CargoManagement.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> Get()
        {
            return new CMSResponse().Ok(await _userService.Get());
        }

        [HttpGet("PaginatedList")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetDetailsAsQueryable([FromHeader] int PageNumber = 1, [FromHeader] int PageSize = 10,[FromQuery] int? hubId=null)
        {
            var res = _userService.GetDetailsAsQueryable(hubId).PaginatedResponse(PageNumber, PageSize);
            return new CMSResponse().Ok(res);
        }

        [HttpGet("Drivers")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetDrivers()
        {
            return new CMSResponse().Ok(await _userService.GetDrivers());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Create(UserCreationDTO user)
        {
            return new CMSResponse().Created(await _userService.Create(user));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int userId)
        {
            var res = await _userService.Delete(userId);
            if (res)
            {
               return new CMSResponse().Ok(res);
            }
            else
            {
                return new CMSResponse().NotFound(null);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Update(UserUpdateDTO user)
        {
            var updatedUser = await _userService.Update(user);
            if (updatedUser != null)
            {
                return new CMSResponse().Ok(updatedUser);
            }
            else
            {
                return new CMSResponse().NotFound(null);
            }
        }
    }
}
