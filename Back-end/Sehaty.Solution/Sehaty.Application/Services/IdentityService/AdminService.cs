namespace Sehaty.Application.Services.IdentityService
{
    public class AdminService(IUnitOfWork unit, IMapper mapper) : IAdminService
    {
        public async Task<Result<IEnumerable<AppUserDto>>> GetAllUsersWithRolesAsync()
        {
            var users = await unit.Users.GetAllWithRolesAsync();

            if (users == null)
                return Result<IEnumerable<AppUserDto>>.Failure(ErrorType.NotFound, "There Is no Users Yet");

            var usersDtos = mapper.Map<IEnumerable<AppUserDto>>(users);

            //var userDtos = users.Select(result => new AppUserDto
            //{
            //    Id = result.user.Id,
            //    UserName = result.user.UserName,
            //    Email = result.user.Email,
            //    FirstName = result.user.FirstName,
            //    LastNAme = result.user.LastName,
            //    PhoneNumber = result.user.PhoneNumber,
            //    Role = result.role
            //}).ToList();
            return Result<IEnumerable<AppUserDto>>.Success(usersDtos);

        }

        public async Task<Result<AppUserDto>> GetUserWithRolesByIdAsync(int userId)
        {
            (ApplicationUser user, string role) user = await unit.Users.GetByIdWithRolesAsync(userId);
            if (user == (null, null))
                return Result<AppUserDto>.Failure(ErrorType.NotFound, "User Not Found");


            var userDto = mapper.Map<AppUserDto>(user);
            /*new AppUserDto
        {
            Id = user.user.Id,
            UserName = user.user.UserName,
            Email = user.user.Email,
            FirstName = user.user.FirstName,
            LastNAme = user.user.LastName,
            PhoneNumber = user.user.PhoneNumber,
            Role = user.role
        };
            */
            return Result<AppUserDto>.Success(userDto);
        }

    }
}
