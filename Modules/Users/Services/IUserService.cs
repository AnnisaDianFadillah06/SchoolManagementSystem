using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Modules.Users.Dtos;

namespace SchoolManagementSystem.Modules.Users.Services
{
    public interface IUserService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    }
}