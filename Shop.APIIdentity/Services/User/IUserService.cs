namespace Shop.APIIdentity.Services.User
{
    public interface IUserService
    {
        Task<bool> UpdatePassword(string userId, string currentPassword, string newPassword);
    }
}
