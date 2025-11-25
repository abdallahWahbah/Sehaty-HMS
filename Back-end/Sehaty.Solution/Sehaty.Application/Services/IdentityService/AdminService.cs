
namespace Sehaty.Application.Services.IdentityService
{
    public class AdminService(IUnitOfWork unit) : IAdminService
    {
        public async Task<IEnumerable<AppUserDto>> GetAllUsersWithRolesAsync()
        {
            var users = await unit.Users.GetAllWithRolesAsync();

            var userDtos = users.Select(result => new AppUserDto
            {
                Id = result.user.Id,
                UserName = result.user.UserName,
                Email = result.user.Email,
                PhoneNumber = result.user.PhoneNumber,
                Role = result.role
            }).ToList();
            return userDtos;
        }

        public async Task<AppUserDto> GetUserWithRolesByIdAsync(int userId)
        {
            var user = await unit.Users.GetByIdWithRolesAsync(userId);
            if (user == (null, null))
            {
                throw new Exception($"User with ID {userId} not found.");
            }
            var userDto = new AppUserDto
            {
                Id = user.user.Id,
                UserName = user.user.UserName,
                Email = user.user.Email,
                PhoneNumber = user.user.PhoneNumber,
                Role = user.role
            };

            return userDto;
        }

    }
}
