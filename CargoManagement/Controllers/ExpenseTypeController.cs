using CargoManagement.Models.ExpenseType;
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
    public class ExpenseTypeController : ControllerBase
    {

        private readonly IExpenseTypeService _service;

        public ExpenseTypeController(IExpenseTypeService hubSerice)
        {
            _service = hubSerice;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Create(ExpenseTypeCreationDTO entry)
        {
            var res = await _service.Create(entry);
            return new CMSResponse().Created(res);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> Update(ExpenseTypeCreationDTO entry)
        {
            var res = await _service.Update(entry);
            return new CMSResponse().Created(res);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int expenseTypeId)
        {
            var res = await _service.Delete(expenseTypeId);
            if (res)
            {
                return new CMSResponse().Ok("Expense type deleted");
            }
            else
            {
                return new CMSResponse().NotFound("Expense type not found");
            }
        }

        [HttpGet("GetPaymentStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel>> GetPaymentStatus()
        {
            var res = await _service.GetPaymentStatus();
            return new CMSResponse().Ok(res);
        }
    }
}
