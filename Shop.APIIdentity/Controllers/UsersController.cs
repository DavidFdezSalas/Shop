using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.APIIdentity.Dto.Users;
using Shop.APIIdentity.Services.User;

namespace Shop.APIIdentity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPatch("{userId}/change-password")]
        public async Task<ActionResult<PasswordChageResponse>> ChangePassword(String userId, PasswordChageRequest request)
        {
            var result = await _userService.UpdatePassword(userId, request.CurrentPassword, request.NewPassword);

            var response = new PasswordChageResponse { Success = result };

            if (!result)
            {
                return BadRequest(response);
            }

            return Ok(response);

        }
    }
}
