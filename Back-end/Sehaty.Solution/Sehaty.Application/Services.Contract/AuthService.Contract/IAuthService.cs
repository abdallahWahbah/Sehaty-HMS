namespace Sehaty.Application.Services.Contract.AuthService.Contract
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken, string ipAddress);
        Task LogoutAsync(int userId, string refreshToken);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto, string ipAddress);

        Task ResetPasswordAsync(string email, string otp, string newPassword);
        Task<bool> VerifyOptAsync(string email, string code);
        Task RequestResetPasswordAsync(string email);

        Task ResendOtpAsync(string email);

    }
}
