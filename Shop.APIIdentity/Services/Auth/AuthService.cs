using MassTransit;
using Microsoft.AspNetCore.Identity;
using Shop.APIIdentity.Dto.Auth;
using Shop.Shared.Events;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shop.APIIdentity.Services.Auth
{
    public class AuthService : IAuthService
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IPublishEndpoint _publishEndpoint;


        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService, IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ResponseLogin> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResponseLogin
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                return new ResponseLogin
                {
                    Success = false,
                    ErrorMessage = "Invalid credentials."
                };
            }

            // Claims
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "NoRole")
            };

            var tokenResult = _tokenService.GenerateToken(claims);

            return new ResponseLogin
            {
                Success = true,
                Token = tokenResult.Token,
                ExpirationAt = tokenResult.Expiration
            };


        }

        public async Task<bool> Register(string username, string email, string password)
        {
            var result = await _userManager.CreateAsync(new IdentityUser
            {
                UserName = username,
                Email = email
            }, password);

            var user = await _userManager.FindByEmailAsync(email);

            if (user?.Id != null && user.Email != null)
            {
                await _publishEndpoint.Publish(new UserCreatedEvent(user.Id, user.Email));
            }

            return result.Succeeded;
        }
    }
}
