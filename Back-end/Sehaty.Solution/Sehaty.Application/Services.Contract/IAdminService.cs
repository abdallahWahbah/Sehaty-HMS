namespace Sehaty.Application.Services.Contract
{
    public interface IAdminService
    {
        Task<AppUserDto> GetUserWithRolesByIdAsync(int userId);
        Task<IEnumerable<AppUserDto>> GetAllUsersWithRolesAsync();
    }
}
