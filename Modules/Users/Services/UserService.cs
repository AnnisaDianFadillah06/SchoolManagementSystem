using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Configurations;
using SchoolManagementSystem.Modules.Users.Dtos;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Modules.Users.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.Modules.Users.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

      public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            // Validasi username dan email
            if (await _userRepository.ExistsAsync(registerDto.Username, registerDto.Email))
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Username or email already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            // Validasi StudentId
            if (registerDto.StudentId.HasValue)
            {
                if (!await _context.Students.AnyAsync(s => s.Id == registerDto.StudentId))
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse(
                        $"Student with ID {registerDto.StudentId} does not exist",
                        AppConstants.StatusCodes.BadRequest);
                }
                if (await _context.Users.AnyAsync(u => u.StudentId == registerDto.StudentId))
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse(
                        AppConstants.Messages.UserAlreadyExistsForStudent,
                        AppConstants.StatusCodes.BadRequest);
                }
            }

            // Validasi TeacherId
            if (registerDto.TeacherId.HasValue)
            {
                if (!await _context.Teachers.AnyAsync(t => t.Id == registerDto.TeacherId))
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse(
                        $"Teacher with ID {registerDto.TeacherId} does not exist",
                        AppConstants.StatusCodes.BadRequest);
                }
                if (await _context.Users.AnyAsync(u => u.TeacherId == registerDto.TeacherId))
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse(
                        AppConstants.Messages.UserAlreadyExistsForTeacher,
                        AppConstants.StatusCodes.BadRequest);
                }
            }

            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            user.Role = registerDto.Role;
            user.StudentId = registerDto.StudentId;
            user.TeacherId = registerDto.TeacherId;

            var createdUser = await _userRepository.CreateAsync(user);

            var token = GenerateJwtToken(createdUser);
            var refreshToken = GenerateRefreshToken();
            createdUser.RefreshToken = refreshToken;
            createdUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(createdUser);

            var response = new AuthResponseDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Role = createdUser.Role,
                Token = token,
                RefreshToken = refreshToken
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "User registered successfully", AppConstants.StatusCodes.Created);
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username && u.IsActive);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Invalid username or password", 
                    AppConstants.StatusCodes.Unauthorized);
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            var response = new AuthResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                RefreshToken = refreshToken
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful");
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.RefreshToken == refreshTokenDto.RefreshToken && u.IsActive);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Invalid or expired refresh token", 
                    AppConstants.StatusCodes.Unauthorized);
            }

            var token = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            var response = new AuthResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                RefreshToken = newRefreshToken
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully");
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration["JwtSettings:Secret"] ?? Environment.GetEnvironmentVariable("JwtSettings__Secret");
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT Secret is not configured.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("StudentId", user.StudentId?.ToString() ?? string.Empty),
                new Claim("TeacherId", user.TeacherId?.ToString() ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JwtSettings__Issuer"),
                audience: _configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("JwtSettings__Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? Environment.GetEnvironmentVariable("JwtSettings__ExpiryMinutes") ?? "30")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}