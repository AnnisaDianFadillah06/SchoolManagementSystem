using AutoMapper;
using SchoolManagementSystem.Modules.Teachers.Repositories;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Users.Repositories;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Common.Attributes;

namespace SchoolManagementSystem.Modules.Teachers.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepository, IUserRepository userRepository, IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TeacherDto>> GetByIdAsync(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null)
            {
                return ApiResponse<TeacherDto>.ErrorResponse(
                    AppConstants.Messages.TeacherNotFound,
                    AppConstants.StatusCodes.NotFound);
            }

            var teacherDto = _mapper.Map<TeacherDto>(teacher);
            return ApiResponse<TeacherDto>.SuccessResponse(teacherDto);
        }

        public async Task<ApiResponse<PaginatedResponse<TeacherDto>>> GetAllAsync(PaginationRequest request)
        {
            var (teachers, totalCount) = await _teacherRepository.GetAllAsync(request);
            var teacherDtos = _mapper.Map<List<TeacherDto>>(teachers);

            var paginatedResponse = PaginatedResponse<TeacherDto>.Create(
                teacherDtos, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResponse<TeacherDto>>.SuccessResponse(paginatedResponse);
        }

        public async Task<ApiResponse<TeacherDto>> CreateAsync(CreateTeacherDto createDto)
        {
            // Check if email already exists
            if (await _teacherRepository.EmailExistsAsync(createDto.Email))
            {
                return ApiResponse<TeacherDto>.ErrorResponse(
                    "Email already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            // Check if NIP already exists
            if (await _teacherRepository.NIPExistsAsync(createDto.NIP))
            {
                return ApiResponse<TeacherDto>.ErrorResponse(
                    "NIP already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            // Check if user with this email already exists
            if (await _userRepository.ExistsAsync("", createDto.Email))
            {
                return ApiResponse<TeacherDto>.ErrorResponse(
                    "User with this email already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            var teacher = _mapper.Map<Teacher>(createDto);
            var createdTeacher = await _teacherRepository.CreateAsync(teacher);

            // Auto-create user account for the teacher
            await CreateUserForTeacherAsync(createdTeacher);

            var teacherDto = _mapper.Map<TeacherDto>(createdTeacher);

            return ApiResponse<TeacherDto>.SuccessResponse(
                teacherDto,
                "Teacher created successfully with default user account",
                AppConstants.StatusCodes.Created);
        }

        public async Task<ApiResponse<TeacherDto>> PatchAsync(int id, PatchTeacherDto patchDto)
        {
            var existingTeacher = await _teacherRepository.GetByIdAsync(id);
            if (existingTeacher == null)
            {
                return ApiResponse<TeacherDto>.ErrorResponse(
                    AppConstants.Messages.TeacherNotFound,
                    AppConstants.StatusCodes.NotFound);
            }

            if (!string.IsNullOrWhiteSpace(patchDto.Email) &&
                await _teacherRepository.EmailExistsAsync(patchDto.Email, id))
            {
                return ApiResponse<TeacherDto>.ErrorResponse(
                    "Email already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            if (!string.IsNullOrWhiteSpace(patchDto.NIP) &&
                await _teacherRepository.NIPExistsAsync(patchDto.NIP, id))
            {
                return ApiResponse<TeacherDto>.ErrorResponse(
                    "NIP already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            // Manual update (partial)
            if (!string.IsNullOrWhiteSpace(patchDto.NIP)) existingTeacher.NIP = patchDto.NIP;
            if (!string.IsNullOrWhiteSpace(patchDto.FirstName)) existingTeacher.FirstName = patchDto.FirstName;
            if (!string.IsNullOrWhiteSpace(patchDto.LastName)) existingTeacher.LastName = patchDto.LastName;
            if (!string.IsNullOrWhiteSpace(patchDto.Email)) existingTeacher.Email = patchDto.Email;
            if (!string.IsNullOrWhiteSpace(patchDto.Phone)) existingTeacher.Phone = patchDto.Phone;
            if (!string.IsNullOrWhiteSpace(patchDto.Subject)) existingTeacher.Subject = patchDto.Subject;
            if (!string.IsNullOrWhiteSpace(patchDto.Qualification)) existingTeacher.Qualification = patchDto.Qualification;

            existingTeacher.UpdatedAt = DateTime.UtcNow;

            var updated = await _teacherRepository.PatchAsync(existingTeacher);
            var dto = _mapper.Map<TeacherDto>(updated);

            return ApiResponse<TeacherDto>.SuccessResponse(
                dto,
                AppConstants.Messages.TeacherUpdated);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            if (!await _teacherRepository.ExistsAsync(id))
            {
                return ApiResponse<bool>.ErrorResponse(
                    AppConstants.Messages.TeacherNotFound,
                    AppConstants.StatusCodes.NotFound);
            }

            var result = await _teacherRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(
                result,
                AppConstants.Messages.TeacherDeleted);
        }

        private async Task CreateUserForTeacherAsync(Teacher teacher)
        {
            // Generate username from email (part before @)
            var username = teacher.Email.Split('@')[0];
            
            // Ensure username is unique
            var originalUsername = username;
            int counter = 1;
            while (await _userRepository.ExistsAsync(username, ""))
            {
                username = $"{originalUsername}{counter}";
                counter++;
            }

            // Default password: "Teacher123!"
            var defaultPassword = "Teacher123!";

            var user = new User
            {
                Username = username,
                Email = teacher.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                Role = UserRoles.Teacher,
                StudentId = null, // Must be null for teacher
                TeacherId = teacher.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
        }
    }
}