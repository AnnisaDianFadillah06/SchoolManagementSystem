using AutoMapper;
using SchoolManagementSystem.Modules.Students.Services;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Users.Repositories;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Common.Attributes;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IUserRepository userRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<StudentDto>> GetByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                return ApiResponse<StudentDto>.ErrorResponse(
                    AppConstants.Messages.StudentNotFound, 
                    AppConstants.StatusCodes.NotFound);
            }

            var studentDto = _mapper.Map<StudentDto>(student);
            return ApiResponse<StudentDto>.SuccessResponse(studentDto);
        }

        public async Task<ApiResponse<PaginatedResponse<StudentDto>>> GetAllAsync(PaginationRequest request)
        {
            var (students, totalCount) = await _studentRepository.GetAllAsync(request);
            var studentDtos = _mapper.Map<List<StudentDto>>(students);
            
            var paginatedResponse = PaginatedResponse<StudentDto>.Create(
                studentDtos, totalCount, request.Page, request.PageSize);
            
            return ApiResponse<PaginatedResponse<StudentDto>>.SuccessResponse(paginatedResponse);
        }

        public async Task<ApiResponse<StudentDto>> CreateAsync(CreateStudentDto createDto)
        {
            // Check if email already exists
            if (await _studentRepository.EmailExistsAsync(createDto.Email))
            {
                return ApiResponse<StudentDto>.ErrorResponse(
                    "Email already exists", 
                    AppConstants.StatusCodes.BadRequest);
            }

            // Check if user with this email already exists
            if (await _userRepository.ExistsAsync("", createDto.Email))
            {
                return ApiResponse<StudentDto>.ErrorResponse(
                    "User with this email already exists", 
                    AppConstants.StatusCodes.BadRequest);
            }

            var student = _mapper.Map<Student>(createDto);
            var createdStudent = await _studentRepository.CreateAsync(student);

            // Auto-create user account for the student
            await CreateUserForStudentAsync(createdStudent);

            var studentDto = _mapper.Map<StudentDto>(createdStudent);
            
            return ApiResponse<StudentDto>.SuccessResponse(
                studentDto, 
                "Student created successfully with default user account", 
                AppConstants.StatusCodes.Created);
        }

        public async Task<ApiResponse<StudentDto>> PatchAsync(int id, PatchStudentDto patchDto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null)
            {
                return ApiResponse<StudentDto>.ErrorResponse(
                    AppConstants.Messages.StudentNotFound,
                    AppConstants.StatusCodes.NotFound);
            }

            if (!string.IsNullOrWhiteSpace(patchDto.Email) &&
                await _studentRepository.EmailExistsAsync(patchDto.Email, id))
            {
                return ApiResponse<StudentDto>.ErrorResponse(
                    "Email already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            // Partial update manual mapping
            if (!string.IsNullOrWhiteSpace(patchDto.NISN)) existingStudent.NISN = patchDto.NISN;
            if (!string.IsNullOrWhiteSpace(patchDto.FirstName)) existingStudent.FirstName = patchDto.FirstName;
            if (!string.IsNullOrWhiteSpace(patchDto.LastName)) existingStudent.LastName = patchDto.LastName;
            if (!string.IsNullOrWhiteSpace(patchDto.Email)) existingStudent.Email = patchDto.Email;
            if (!string.IsNullOrWhiteSpace(patchDto.Phone)) existingStudent.Phone = patchDto.Phone;
            if (patchDto.DateOfBirth.HasValue) existingStudent.DateOfBirth = patchDto.DateOfBirth.Value;
            if (!string.IsNullOrWhiteSpace(patchDto.Address)) existingStudent.Address = patchDto.Address;

            existingStudent.UpdatedAt = DateTime.UtcNow;

            var updated = await _studentRepository.PatchAsync(existingStudent);
            var dto = _mapper.Map<StudentDto>(updated);

            return ApiResponse<StudentDto>.SuccessResponse(dto, AppConstants.Messages.StudentUpdated);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            if (!await _studentRepository.ExistsAsync(id))
            {
                return ApiResponse<bool>.ErrorResponse(
                    AppConstants.Messages.StudentNotFound, 
                    AppConstants.StatusCodes.NotFound);
            }

            var result = await _studentRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(
                result, 
                AppConstants.Messages.StudentDeleted);
        }

        private async Task CreateUserForStudentAsync(Student student)
        {
            // Generate username from email (part before @)
            var username = student.Email.Split('@')[0];
            
            // Ensure username is unique
            var originalUsername = username;
            int counter = 1;
            while (await _userRepository.ExistsAsync(username, ""))
            {
                username = $"{originalUsername}{counter}";
                counter++;
            }

            // Default password: "Student123!"
            var defaultPassword = "Student123!";

            var user = new User
            {
                Username = username,
                Email = student.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                Role = UserRoles.Student,
                StudentId = student.Id,
                TeacherId = null, // Must be null for student
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
        }
    }
}