using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.APIIdentity.Dto.Auth;

namespace Shop.APIIdentity.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> Register(string username, string email, string password);
        Task<ResponseLogin> Login(string email, string password);
    }
}
