using Microsoft.AspNetCore.Identity;
using Shop.APIIdentity.Dto.Users;

namespace Shop.APIIdentity.Services.User
{
    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<IdentityUser> userManager, ILogger<UserService> logger, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<bool> UpdatePassword(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to change password for user ID {UserId}. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("Password changed successfully for user ID {UserId}.", userId);
            return true;
        }

        public async Task<UserInfoResponse?> GetCurrentUserInfo(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserInfoResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "NoRole"
            };
        }

        public async Task<bool> UpdateUserInfo(string userId, string newUserName, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }

            user.UserName = newUserName;
            user.Email = newEmail;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to update user info for user ID {UserId}. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("User info updated successfully for user ID {UserId}.", userId);
            return true;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to delete user ID {UserId}. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("User with ID {UserId} deleted successfully.", userId);
            return true;
        }

        public async Task<GetUsersResponse> GetAllUsers(int pageNumber, int pageSize)
        {
            var totalUsers = _userManager.Users.Count();

            var users = _userManager.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userInfoList = new List<UserInfoResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userInfoList.Add(new UserInfoResponse
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "NoRole"
                });
            }

            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            return new GetUsersResponse
            {
                Users = userInfoList,
                TotalCount = totalUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<bool> LockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to lock user ID {UserId}. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("User with ID {UserId} locked successfully.", userId);
            return true;
        }

        public async Task<bool> UnlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to unlock user ID {UserId}. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("User with ID {UserId} unlocked successfully.", userId);
            return true;
        }

        public async Task<bool> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }

            // Verificar que el rol existe
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                _logger.LogWarning("Role '{RoleName}' does not exist.", roleName);
                return false;
            }

            // Quitar roles anteriores
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Asignar nuevo rol
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to assign role '{RoleName}' to user ID {UserId}. Errors: {Errors}", 
                    roleName, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("Role '{RoleName}' assigned to user ID {UserId} successfully.", roleName, userId);
            return true;
        }

        public async Task<UserStatsResponse> GetUserStats()
        {
            var allUsers = _userManager.Users.ToList();
            var totalUsers = allUsers.Count;
            var lockedUsers = allUsers.Count(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow);

            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var customerRole = await _roleManager.FindByNameAsync("Customer");

            var totalAdmins = 0;
            var totalCustomers = 0;

            if (adminRole != null)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                totalAdmins = admins.Count;
            }

            if (customerRole != null)
            {
                var customers = await _userManager.GetUsersInRoleAsync("Customer");
                totalCustomers = customers.Count;
            }

            return new UserStatsResponse
            {
                TotalUsers = totalUsers,
                TotalAdmins = totalAdmins,
                TotalCustomers = totalCustomers,
                LockedUsers = lockedUsers
            };
        }
    }
}
