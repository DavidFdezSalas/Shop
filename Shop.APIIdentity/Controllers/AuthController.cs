using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Shop.APIIdentity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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

        //[HttpPost("login")]
        //public async Task<IActionResult<UserResponse>> Login(User u)
        //{
        //    // Login logic goes here
        //    var userExists = await _userManager.FindByNameAsync
        //    if (userExists == null)
        //    {
        //        return Unauthorized("Invalid username or password.");
        //    }
        //    return Ok("User logged in successfully.");
        //}
        

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
