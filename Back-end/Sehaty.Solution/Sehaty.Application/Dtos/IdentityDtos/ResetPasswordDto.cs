namespace Sehaty.Application.Dtos.IdentityDtos
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
