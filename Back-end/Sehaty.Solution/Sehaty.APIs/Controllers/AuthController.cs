using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.IdentityDtos;
using Sehaty.Application.Services.Contract.AuthService.Contract;
using System.Security.Claims;

namespace Sehaty.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                var result = await authService.RegisterAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                model.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await authService.LoginAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await authService.ChangePasswordAsync(userId, model, ipAddress);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto model)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await authService.RefreshTokenAsync(model.Token, model.RefreshToken, ipAddress);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutDto model)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await authService.LogoutAsync(userId, model.RefreshToken);
                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("request-password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPasswordDto model)
        {
            try
            {
                await authService.RequestResetPasswordAsync(model.Email);
                return Ok(new { message = "Password reset OTP has been sent to your email." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var isValid = await authService.VerifyOptAsync(dto.Email, dto.Otp);
            return Ok(new { isValid });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                await authService.ResetPasswordAsync(model.Email, model.Otp, model.NewPassword);
                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto model)
        {
            try
            {
                await authService.ResendOtpAsync(model.Email);
                return Ok(new { message = "OTP has been resent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
