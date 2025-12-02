using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.APIIdentity.Dto.Users;
using Shop.APIIdentity.Services.User;

namespace Shop.APIIdentity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserService userService, ILogger<AdminController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("Users")]
        public async Task<ActionResult<GetUsersResponse>> GetAllUsers([FromQuery] GetUsersRequest request)
        {
            var response = await _userService.GetAllUsers(request.PageNumber, request.PageSize);
            return Ok(response);
        }

        [HttpGet("Users/{userId}")]
        public async Task<ActionResult<UserInfoResponse>> GetUserById(string userId)
        {
            var userInfo = await _userService.GetCurrentUserInfo(userId);

            if (userInfo == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            return Ok(userInfo);
        }

        [HttpPut("Users/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UpdateUserInfoRequest request)
        {
            var result = await _userService.UpdateUserInfo(userId, request.UserName, request.Email);

            if (!result)
            {
                return BadRequest($"Failed to update user with ID {userId}.");
            }

            return Ok("User information updated successfully.");
        }

        [HttpDelete("Users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUser(userId);

            if (!result)
            {
                return BadRequest($"Failed to delete user with ID {userId}.");
            }

            return Ok("User deleted successfully.");
        }

        [HttpPatch("Users/{userId}/lock")]
        public async Task<IActionResult> LockUser(string userId)
        {
            var result = await _userService.LockUser(userId);

            if (!result)
            {
                return BadRequest($"Failed to lock user with ID {userId}.");
            }

            return Ok("User locked successfully.");
        }

        [HttpPatch("Users/{userId}/unlock")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var result = await _userService.UnlockUser(userId);

            if (!result)
            {
                return BadRequest($"Failed to unlock user with ID {userId}.");
            }

            return Ok("User unlocked successfully.");
        }

        [HttpPost("Users/{userId}/roles")]
        public async Task<IActionResult> AssignRole(string userId, AssignRoleRequest request)
        {
            var result = await _userService.AssignRole(userId, request.RoleName);

            if (!result)
            {
                return BadRequest($"Failed to assign role '{request.RoleName}' to user with ID {userId}.");
            }

            return Ok($"Role '{request.RoleName}' assigned successfully.");
        }

        [HttpGet("Users/stats")]
        public async Task<ActionResult<UserStatsResponse>> GetUserStats()
        {
            var stats = await _userService.GetUserStats();
            return Ok(stats);
        }
    }
}
