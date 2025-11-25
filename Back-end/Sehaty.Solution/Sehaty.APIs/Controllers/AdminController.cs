namespace Sehaty.APIs.Controllers
{

    public class AdminController(IAdminService adminService) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetAllUSers()
        {
            var users = await adminService.GetAllUsersWithRolesAsync();
            if (users == null)
                return NotFound(new ApiResponse(404));
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUserDto>> GetUserDataById(int id)
        {
            var user = await adminService.GetUserWithRolesByIdAsync(id);
            if (user == null)
                return NotFound(new ApiResponse(404));
            return Ok(user);
        }
    }
}
