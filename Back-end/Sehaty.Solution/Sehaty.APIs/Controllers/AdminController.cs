namespace Sehaty.APIs.Controllers
{

    public class AdminController(IAdminService adminService) : ApiBaseController
    {
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetAllUsers()
        {
            var result = await adminService.GetAllUsersWithRolesAsync();
            if (!result.IsSuccess)
                return NotFound(new ApiResponse(404, result.Error));
            //return result.ToApiResponse();
            var users = result.Data;
            return Ok(users);
        }
        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<AppUserDto>> GetUserDataById(int id)
        {
            var result = await adminService.GetUserWithRolesByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new ApiResponse(404, result.Error));
            var user = result.Data;
            return Ok(user);
        }
    }
}
