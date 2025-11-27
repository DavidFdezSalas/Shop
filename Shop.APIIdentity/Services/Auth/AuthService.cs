using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shop.APIIdentity.Dto.Auth;
using Shop.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shop.APIIdentity.Services.Auth
{
    public class AuthService : IAuthService
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
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

            // Generate JWT Token
            var secretKey = _configuration["JwtSettings:Key"]!;
            var audience = _configuration["JwtSettings:Audience"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpiryInMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationTime = DateTime.UtcNow.AddMinutes(expirationMinutes);


            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expirationTime,
                signingCredentials: creds
            );

            var encryptedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new ResponseLogin
            {
                Success = true,
                Token = encryptedToken,
                ExpirationAt = expirationTime
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
                await _publishEndpoint.Publish(new UserCreatedEvents(user.Id, user.Email));
            }

            return result.Succeeded;
        }
    }
}
