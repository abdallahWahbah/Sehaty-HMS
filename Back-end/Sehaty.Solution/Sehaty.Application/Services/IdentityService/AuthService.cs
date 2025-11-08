using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sehaty.Application.Dtos.IdentityDtos;
using Sehaty.Application.Services.Contract.AuthService.Contract;
using Sehaty.Application.Shared.AuthShared;
using Sehaty.Core.Entities.User_Entities;
using Sehaty.Infrastructure.Data.Contexts;
using Sehaty.Infrastructure.Service.Email;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.IdentityService
{
    public class AuthService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<JwtOptions> options, SehatyDbContext context, IEmailSender emailSender) : IAuthService
    {

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
                throw new Exception("Password And Confirm Password Do Not Match");

            var existingEmail = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingEmail is not null)
                throw new Exception("Email already exists");

            var existingUserName = await userManager.FindByNameAsync(registerDto.UserName);
            if (existingUserName != null)
                throw new Exception("Username already exists");

            var defaultRole = await roleManager.FindByNameAsync("Patient");
            if (defaultRole is null)
                throw new Exception("Default Role 'Patient' not found. Please seed roles.");

            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                LanguagePreference = registerDto.LanguagePreference,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User Registration Failed: {errors}");
            }

            // إزالة أي أدوار موجودة (إذا كان فيه أي رول سابق)
            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // إضافة الدور الافتراضي
            await userManager.AddToRoleAsync(user, defaultRole.Name);

            // جلب الدور الحالي بعد الإضافة
            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? defaultRole.Name;

            // إنشاء JWT
            var token = await GenerateJwtTokenAsync(user, userRole);
            var jwtOptions = options.Value;

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationInMinutes),
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRole,
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.Users.Include(r=> r.RefreshTokens).FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (user is null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new Exception("Invalid UserName Or Password");
            }
            user.LastLogin = DateTime.Now;
            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Patient";
            var token = await GenerateJwtTokenAsync(user, userRole);

            var refreshToken = await AddRefreshTokenAsync(user, loginDto.IpAddress);
            var jwtOptions = options.Value;
            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationInMinutes),
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRole,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.Expires,

            };

        }
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role)
            };
            var jwtOptions = options.Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationInMinutes),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private RefreshToken CreateRefreshToken(string? createByIp = null)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var jwtOptions = options.Value;
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenExpirationInDays),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = createByIp,
                IsRevoked = false
            };
        }
        private async Task<RefreshToken> AddRefreshTokenAsync(ApplicationUser user, string? ipAddress)
        {
            var refreshToken = CreateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);
            return refreshToken;
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken, string? ipAdrees)
        {
            var user = await userManager.Users.Include(r => r.RefreshTokens)
                .FirstOrDefaultAsync(u=> u.RefreshTokens.Any(t=> t.Token == refreshToken));
            if (user is null)
            {
                throw new Exception("Invalid Refresh Token");
            }
            var existingRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

            if(existingRefreshToken is null || existingRefreshToken.IsRevoked || existingRefreshToken.Expires <= DateTime.UtcNow)
            {
                throw new Exception("Invalid or Expired Refresh Token");
            }
            existingRefreshToken.IsRevoked = true;
            var roles = await userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Patient";
            var newAccessToken = await GenerateJwtTokenAsync(user, userRole);

            var newRefreshToken =  CreateRefreshToken(ipAdrees);
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Token = newAccessToken,
                Expiration = DateTime.UtcNow.AddDays(10),
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRole,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.Expires
            };

        }
        public async Task LogoutAsync(int userId, string refreshToken)
        {
            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) 
            { 
                throw new Exception("User Not Found");
            }
            var existingRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if(existingRefreshToken is null)
            {
                throw new Exception("Invalid refresh token");
            }
            existingRefreshToken.IsRevoked = true;
            existingRefreshToken.RevokedAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
        }
        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto, string? ipAddress)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                throw new Exception("User Not Found");
            }
            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                throw new Exception("New Password And Confirm New Password Do Not Match");
            }
            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Password Change Failed: {errors}");
            }
            var audit = new AuditLog
            {
                UserId = user.Id,
                Action = "Change Password",
                IpAdress = ipAddress,
                CreatAt = DateTime.UtcNow
            };
        }
        private string GenerateOtp()
        {
            var randomNumber = new Random();
            return randomNumber.Next(100000, 999999).ToString();
        }
        public async Task RequestResetPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user is null)
            {
                throw new Exception("User Not Found");
            }
            var otp = GenerateOtp();
            var otpEntry = new PasswordResetCode
            {
                UserId = user.Id,
                CodeHash = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };
            context.PasswordResetCodes.Add(otpEntry);
            await context.SaveChangesAsync();
            await emailSender.SendEmailAsync(user.Email!,
                "Password Reset Code",
                $@"
    <div style='
        font-family: Arial, sans-serif;
        background-color: #f7f9fc;
        padding: 20px;
        border-radius: 10px;
        max-width: 400px;
        margin: auto;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        text-align: center;
        color: #333;
    '>
        <h2 style='color: #2c3e50;'>Password Reset</h2>
        <p style='font-size: 16px;'>
            You requested to reset your password.<br/>
            Use the code below to proceed:
        </p>
        <p style='
            font-size: 28px;
            font-weight: bold;
            background-color: #3498db;
            color: white;
            padding: 10px 0;
            border-radius: 5px;
            letter-spacing: 5px;
            margin: 20px 0;
            '>
            {otp}
        </p>
        <p style='font-size: 14px; color: #777;'>
            If you didn't request this, please ignore this email.
        </p>
    </div>
    ");
        }
        public async Task<bool> VerifyOptAsync(string email, string code) 
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user is null)
            {
                return false;
            }
            var otpEntry = await context.PasswordResetCodes
                .Where(u => u.UserId == user.Id && u.CodeHash == code && !u.IsUsed)
                .OrderByDescending(u => u.CreatedAt)
                .FirstOrDefaultAsync();
            if(otpEntry is null || otpEntry.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }
            return true;
        }
        public async Task ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user is null)
            {
                throw new Exception("Invaild Request!");
            }
            var otpEntry = await context.PasswordResetCodes
                .Where(u => u.UserId == user.Id && u.CodeHash == otp && !u.IsUsed)
                .OrderByDescending(u => u.CreatedAt)
                .FirstOrDefaultAsync();
            if (otpEntry is null || otpEntry.ExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Invaild Or Expired OTP Code!");
            }
            var remove = await userManager.RemovePasswordAsync(user);
            if (!remove.Succeeded)
            {
               
                throw new Exception($"Faild to reset Password");
            }
            var add = await userManager.AddPasswordAsync(user, newPassword);
            if (!add.Succeeded)
            {
                throw new Exception($"Faild to reset new Password");
            }
            otpEntry.IsUsed = true;
            await context.SaveChangesAsync();
            context.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                Action = "Reset Password",
                CreatAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
        public async Task ResendOtpAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new Exception("User Not Found");
            }
            var lastOtp = await context.PasswordResetCodes
                .Where(u => u.UserId == user.Id)
                .OrderByDescending(u => u.CreatedAt)
                .FirstOrDefaultAsync();
            if(lastOtp != null && (DateTime.UtcNow - lastOtp.CreatedAt).TotalMinutes < 1)
            {
                throw new Exception("Please wait before requesting a new OTP.");
            }
            var otp = GenerateOtp();
            var otpEntry = new PasswordResetCode
            {
                UserId = user.Id,
                CodeHash = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };
            context.PasswordResetCodes.Add(otpEntry);
            await context.SaveChangesAsync();
            await emailSender.SendEmailAsync(user.Email!,
                "Password Reset Code",
                $@"
    <div style='
        font-family: Arial, sans-serif;
        background-color: #f7f9fc;
        padding: 20px;
        border-radius: 10px;
        max-width: 400px;
        margin: auto;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        text-align: center;
        color: #333;
    '>
        <h2 style='color: #2c3e50;'>Password Reset</h2>
        <p style='font-size: 16px;'>
            You requested to reset your password.<br/>
            Use the code below to proceed:
        </p>
        <p style='
            font-size: 28px;
            font-weight: bold;
            background-color: #3498db;
            color: white;
            padding: 10px 0;
            border-radius: 5px;
            letter-spacing: 5px;
            margin: 20px 0;
            '>
            {otp}
        </p>
        <p style='font-size: 14px; color: #777;'>
            If you didn't request this, please ignore this email.
        </p>
    </div>
    ");
        }
    }
}
