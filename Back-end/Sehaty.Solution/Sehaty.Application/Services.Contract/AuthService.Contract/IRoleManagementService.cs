namespace Sehaty.Application.Services.Contract.AuthService.Contract
{
    public interface IRoleManagementService
    {
        Task<string> ChangeUserRoleAsync(int userId, int newRoleId);
        Task<string> GetUserRoleAsync(int userId);
    }
}
