namespace Sehaty.Application.Dtos.IdentityDtos
{
    public class AppUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; }

        public IList<string> Roles { get; set; }

    }
}
