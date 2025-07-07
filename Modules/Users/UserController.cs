using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Modules.Users.Dtos;
using SchoolManagementSystem.Modules.Users.Services;

namespace SchoolManagementSystem.Modules.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _userService.RegisterAsync(registerDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var response = await _userService.LoginAsync(loginDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var response = await _userService.RefreshTokenAsync(refreshTokenDto);
            return StatusCode(response.StatusCode, response);
        }
    }
}