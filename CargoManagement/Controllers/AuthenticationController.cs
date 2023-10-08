using CargoManagement.Models.Authentication;
using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("validateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseModel>> Validateuser(AuthenticationRequestDTO user)
        {
            var res = await _service.ValidateUser(user);
            if (res != null)
            {
                return (new CMSResponse()).Ok(res);
            }
            return (new CMSResponse()).Unauthorized("Invalid Credentials");
        }

        [HttpPost("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<ResponseModel>> ChangePassword(ChangePasswordDTO model)
        {
            var res = await _service.ChangePassword(model.currentPassword, model.newPassword);
            if (res)
            {
                return (new CMSResponse()).Ok(res);
            }
            return (new CMSResponse()).BadRequest("Invalid Current password");
        }

        [HttpPost("UpdateProfilePic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<ResponseModel>> UpdateProfilePic(string newImage)
        {
            var res = await _service.UpdateProfilePic(newImage);
            if (res)
            {
                return (new CMSResponse()).Ok(res);
            }
            return (new CMSResponse()).BadRequest("Invalid User");
        }

        [HttpPost("UpdateProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<ResponseModel>> UpdateProfile([FromBody] UpdateProfileDTO user)
        {
            var res = await _service.UpdateProfile(user);
            if (res)
            {
                return (new CMSResponse()).Ok(res);
            }
            return (new CMSResponse()).BadRequest("Invalid User");
        }

        [HttpGet("MyProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<ResponseModel>> MyProfile()
        {
            var res = await _service.MyProfile();
            if (res != null)
            {
                return (new CMSResponse()).Ok(res);
            }
            return (new CMSResponse()).Unauthorized("Invalid User");
        }
    }
}
