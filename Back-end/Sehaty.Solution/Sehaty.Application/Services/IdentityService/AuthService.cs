using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sehaty.Application.Dtos.IdentityDtos;
using Sehaty.Application.Services.Contract.AuthService.Contract;
using Sehaty.Application.Shared.AuthShared;
using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.IdentityService
{
    public class AuthService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<JwtOptions> options) : IAuthService
    {

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new Exception("Password And Confirm Password Do Not Match");
            }
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
                RoleId = defaultRole.Id,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
            };
            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User Registration Failed: {errors}");
            }
            await userManager.AddToRoleAsync(user, defaultRole.Name);
            var token = await GenerateJwtTokenAsync(user, defaultRole.Name);
            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(10),
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = defaultRole.Name,
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.Users.Include(r=> r.Role).FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (user is null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new Exception("Invalid UserName Or Password");
            }
            user.LastLogin = DateTime.Now;
            await userManager.UpdateAsync(user);
            var token = await GenerateJwtTokenAsync(user, user.Role.Name);
            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(10),
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.Name,
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
                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
