using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sehaty.Application.Services.Contract.AuthService.Contract;
using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.IdentityService
{
    public class RoleManagementService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IRoleManagementService
    {
        public async Task<string> ChangeUserRoleAsync(int userId, int newRoleId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                throw new Exception("User not found");
            var newRole = await roleManager.FindByIdAsync(newRoleId.ToString());
            if (newRole is null)
                throw new Exception("Role not found");
            var currentRole = await userManager.GetRolesAsync(user);
            if (currentRole.Any())
            {
                await userManager.RemoveFromRolesAsync(user, currentRole);
            }
            var addRoleResult = await userManager.AddToRoleAsync(user, newRole.Name);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new Exception($"Role Assignment Failed: {errors}");
            }
            var roles = await userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? string.Empty;
        }

        public async Task<string> GetUserRoleAsync(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new Exception("User not found!");
            var roles = await userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? string.Empty;
        }
    }
}
