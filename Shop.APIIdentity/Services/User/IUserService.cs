using Shop.APIIdentity.Dto.Users;

namespace Shop.APIIdentity.Services.User
{
    public interface IUserService
    {
        Task<bool> UpdatePassword(string userId, string currentPassword, string newPassword);
        Task<UserInfoResponse?> GetCurrentUserInfo(string userId);
        Task<bool> UpdateUserInfo(string userId, string newUserName, string newEmail);
        Task<bool> DeleteUser(string userId);
        Task<GetUsersResponse> GetAllUsers(int pageNumber, int pageSize);
        Task<bool> LockUser(string userId);
        Task<bool> UnlockUser(string userId);
        Task<bool> AssignRole(string userId, string roleName);
        Task<UserStatsResponse> GetUserStats();
    }
}
