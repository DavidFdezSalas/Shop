using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.APIIdentity.Dto.Auth;
using Shop.APIIdentity.Services;

namespace Shop.APIIdentity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private IAuthService _authService;

        public AuthController(UserManager<IdentityUser> userManager, IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRequest request)
        {
            var user = new IdentityUser
            {
                UserName = request.Username,
                PasswordHash = request.Password
            };

            var result = await _userManager.CreateAsync(user);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserRequest request)
        {
            var response = await _authService.Login(request.Username, request.Password);
            if (response == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(response);
        }


        public class UserRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class UserResponse
        {
            public required string Username { get; set; }
        }
    }
}
