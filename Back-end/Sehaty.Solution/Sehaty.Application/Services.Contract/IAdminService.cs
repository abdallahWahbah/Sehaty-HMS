namespace Sehaty.Application.Services.Contract
{
    public interface IAdminService
    {
        Task<Result<AppUserDto>> GetUserWithRolesByIdAsync(int userId);
        Task<Result<IEnumerable<AppUserDto>>> GetAllUsersWithRolesAsync();
    }
}
