using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.IdentityDtos;
using Sehaty.Application.Services.Contract.AuthService.Contract;

namespace Sehaty.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController(IRoleManagementService roleService) : ControllerBase
    {
        [HttpPost("ChangeUserRole")]
        public async Task<IActionResult> ChangeUserRole(ChangeUserRoleDto model)
        {
            try
            {
                var result = await roleService.ChangeUserRoleAsync(model.UserId, model.NewRoleId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("GetUserRole/{userId}")]
        public async Task<IActionResult> GetUserRole(int userId)
        {
            try
            {
                var result = await roleService.GetUserRoleAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
