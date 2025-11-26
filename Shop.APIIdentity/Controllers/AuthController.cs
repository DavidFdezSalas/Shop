using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.APIIdentity.Dto.Auth;
using Shop.APIIdentity.Services.Auth;

namespace Shop.APIIdentity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRequest request)
        {
            var user = await _authService.Register(request.Username, request.Email, request.Password);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(RequestLogin request)
        {
            var response = await _authService.Login(request.Email, request.Password);
            if (response == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(response);
        }


        public class UserRequest
        {
            public required string Username { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class UserResponse
        {
            public required string Username { get; set; }
        }
    }
}
