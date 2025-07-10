using AutoMapper;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses; 
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Common.Attributes;

namespace SchoolManagementSystem.Modules.Classes.Services
{

    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository; private readonly IMapper _mapper;

        public ClassService(IClassRepository classRepository, IMapper mapper)
        {
            _classRepository = classRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ClassDto>> GetByIdAsync(int id, int? teacherId, int? studentId, string userRole)
        {
            var classEntity = await _classRepository.GetByIdAsync(id);
            if (classEntity == null)
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    AppConstants.Messages.ClassNotFound,
                    AppConstants.StatusCodes.NotFound);
            }

            // Cek akses berdasarkan peran
            if (userRole == UserRoles.Teacher && classEntity.TeacherId != teacherId)
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    "Access denied: not your class.",
                    AppConstants.StatusCodes.Forbidden);
            }

            if (userRole == UserRoles.Student && !classEntity.Enrollments.Any(e => e.StudentId == studentId))
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    "Access denied: you are not enrolled in this class.",
                    AppConstants.StatusCodes.Forbidden);
            }

            var classDto = _mapper.Map<ClassDto>(classEntity);
            return ApiResponse<ClassDto>.SuccessResponse(classDto);
        }

        public async Task<ApiResponse<PaginatedResponse<ClassDto>>> GetAllAsync(PaginationRequest request)
        {
            var (classes, totalCount) = await _classRepository.GetAllAsync(request);
            var classDtos = _mapper.Map<List<ClassDto>>(classes);

            var paginatedResponse = PaginatedResponse<ClassDto>.Create(
                classDtos, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResponse<ClassDto>>.SuccessResponse(paginatedResponse);
        }

        public async Task<ApiResponse<ClassDto>> CreateAsync(CreateClassDto createDto)
        {
            // Check if class name already exists
            if (await _classRepository.ClassNameExistsAsync(createDto.ClassName))
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    "Class name already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            // Check if teacher exists
            if (!await _classRepository.TeacherExistsAsync(createDto.TeacherId))
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    "Teacher not found",
                    AppConstants.StatusCodes.BadRequest);
            }

            var classEntity = _mapper.Map<Class>(createDto);
            var createdClass = await _classRepository.CreateAsync(classEntity);
            var classDto = _mapper.Map<ClassDto>(createdClass);

            return ApiResponse<ClassDto>.SuccessResponse(
                classDto,
                AppConstants.Messages.ClassCreated,
                AppConstants.StatusCodes.Created);
        }

        public async Task<ApiResponse<ClassDto>> PatchAsync(int id, PatchClassDto patchDto, int? teacherId, string userRole)
        {
            var existingClass = await _classRepository.GetByIdAsync(id);
            if (existingClass == null)
            {
                return ApiResponse<ClassDto>.ErrorResponse("Class not found", 404);
            }

            if (userRole == UserRoles.Teacher && existingClass.TeacherId != teacherId)
            {
                return ApiResponse<ClassDto>.ErrorResponse("Access denied: not your class.", 403);
            }

            if (patchDto.ClassName != null && await _classRepository.ClassNameExistsAsync(patchDto.ClassName, id))
            {
                return ApiResponse<ClassDto>.ErrorResponse("Class name already exists", 400);
            }

            if (patchDto.TeacherId.HasValue && !await _classRepository.TeacherExistsAsync(patchDto.TeacherId.Value))
            {
                return ApiResponse<ClassDto>.ErrorResponse("Teacher not found", 400);
            }

            // Manual update (karena patch hanya sebagian)
            if (patchDto.ClassName != null) existingClass.ClassName = patchDto.ClassName;
            if (patchDto.TeacherId.HasValue) existingClass.TeacherId = patchDto.TeacherId.Value;
            if (patchDto.MaxStudents.HasValue) existingClass.MaxStudents = patchDto.MaxStudents.Value;
            if (patchDto.Schedule != null) existingClass.Schedule = patchDto.Schedule;

            existingClass.UpdatedAt = DateTime.UtcNow;

            var updated = await _classRepository.PatchAsync(existingClass);
            var resultDto = _mapper.Map<ClassDto>(updated);

            return ApiResponse<ClassDto>.SuccessResponse(resultDto, "Class updated successfully.");
        }


        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            if (!await _classRepository.ExistsAsync(id))
            {
                return ApiResponse<bool>.ErrorResponse(
                    AppConstants.Messages.ClassNotFound,
                    AppConstants.StatusCodes.NotFound);
            }

            var result = await _classRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(
                result,
                AppConstants.Messages.ClassDeleted);
        }

    }
}