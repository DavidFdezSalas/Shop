using Microsoft.AspNetCore.Identity;

namespace Shop.APIIdentity.Services.User
{
    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        private ILogger<UserService> _logger;

        public UserService(UserManager<IdentityUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
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
    }
}
