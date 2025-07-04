using AutoMapper;
using SchoolManagementSystem.Modules.Students.Services;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
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

            var student = _mapper.Map<Student>(createDto);
            var createdStudent = await _studentRepository.CreateAsync(student);
            var studentDto = _mapper.Map<StudentDto>(createdStudent);
            
            return ApiResponse<StudentDto>.SuccessResponse(
                studentDto, 
                AppConstants.Messages.StudentCreated, 
                AppConstants.StatusCodes.Created);
        }

        public async Task<ApiResponse<StudentDto>> UpdateAsync(int id, UpdateStudentDto updateDto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null)
            {
                return ApiResponse<StudentDto>.ErrorResponse(
                    AppConstants.Messages.StudentNotFound, 
                    AppConstants.StatusCodes.NotFound);
            }

            // Check if email already exists (excluding current student)
            if (await _studentRepository.EmailExistsAsync(updateDto.Email, id))
            {
                return ApiResponse<StudentDto>.ErrorResponse(
                    "Email already exists", 
                    AppConstants.StatusCodes.BadRequest);
            }

            _mapper.Map(updateDto, existingStudent);
            var updatedStudent = await _studentRepository.UpdateAsync(existingStudent);
            var studentDto = _mapper.Map<StudentDto>(updatedStudent);
            
            return ApiResponse<StudentDto>.SuccessResponse(
                studentDto, 
                AppConstants.Messages.StudentUpdated);
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
    }
}