using AutoMapper;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses; 
using SchoolManagementSystem.Common.Constants;

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

        public async Task<ApiResponse<ClassDto>> GetByIdAsync(int id)
        {
            var classEntity = await _classRepository.GetByIdAsync(id);
            if (classEntity == null)
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    AppConstants.Messages.ClassNotFound,
                    AppConstants.StatusCodes.NotFound);
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

        public async Task<ApiResponse<ClassDto>> UpdateAsync(int id, UpdateClassDto updateDto)
        {
            var existingClass = await _classRepository.GetByIdAsync(id);
            if (existingClass == null)
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    AppConstants.Messages.ClassNotFound,
                    AppConstants.StatusCodes.NotFound);
            }

            // Check if class name already exists (excluding current class)
            if (await _classRepository.ClassNameExistsAsync(updateDto.ClassName, id))
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    "Class name already exists",
                    AppConstants.StatusCodes.BadRequest);
            }

            // Check if teacher exists
            if (!await _classRepository.TeacherExistsAsync(updateDto.TeacherId))
            {
                return ApiResponse<ClassDto>.ErrorResponse(
                    "Teacher not found",
                    AppConstants.StatusCodes.BadRequest);
            }

            _mapper.Map(updateDto, existingClass);
            var updatedClass = await _classRepository.UpdateAsync(existingClass);
            var classDto = _mapper.Map<ClassDto>(updatedClass);

            return ApiResponse<ClassDto>.SuccessResponse(
                classDto,
                AppConstants.Messages.ClassUpdated);
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