using AutoMapper;
using SchoolManagementSystem.Modules.Enrollments.Repositories;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Entities;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Enrollments.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public EnrollmentService(IEnrollmentRepository enrollmentRepository, IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<EnrollmentDto>> GetByIdAsync(int id)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                AppConstants.Messages.EnrollmentNotFound,
                AppConstants.StatusCodes.NotFound);
        }

        var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);
        return ApiResponse<EnrollmentDto>.SuccessResponse(enrollmentDto);
    }

    public async Task<ApiResponse<PaginatedResponse<EnrollmentDto>>> GetAllAsync(PaginationRequest request)
    {
        var (enrollments, totalCount) = await _enrollmentRepository.GetAllAsync(request);
        var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);

        var paginatedResponse = PaginatedResponse<EnrollmentDto>.Create(
            enrollmentDtos, totalCount, request.Page, request.PageSize);

        return ApiResponse<PaginatedResponse<EnrollmentDto>>.SuccessResponse(paginatedResponse);
    }

    public async Task<ApiResponse<EnrollmentDto>> CreateAsync(CreateEnrollmentDto createDto)
    {
        // Check if student exists
        if (!await _enrollmentRepository.StudentExistsAsync(createDto.StudentId))
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                "Student not found",
                AppConstants.StatusCodes.BadRequest);
        }

        // Check if class exists
        if (!await _enrollmentRepository.ClassExistsAsync(createDto.ClassId))
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                "Class not found",
                AppConstants.StatusCodes.BadRequest);
        }

        // Check for duplicate enrollment
        if (await _enrollmentRepository.DuplicateEnrollmentExistsAsync(createDto.StudentId, createDto.ClassId))
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                "Student sudah terdaftar di kelas ini",
                AppConstants.StatusCodes.BadRequest,
                new List<string> { "Duplicate Enrollment" }); // Perbaikan: Gunakan List<string>
        }

        // Check if class has capacity
        if (!await _enrollmentRepository.CanEnrollAsync(createDto.ClassId))
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                "Class has reached maximum capacity",
                AppConstants.StatusCodes.BadRequest);
        }

        var enrollment = _mapper.Map<Enrollment>(createDto);
        var createdEnrollment = await _enrollmentRepository.CreateAsync(enrollment);
        var enrollmentDto = _mapper.Map<EnrollmentDto>(createdEnrollment);

        return ApiResponse<EnrollmentDto>.SuccessResponse(
            enrollmentDto,
            AppConstants.Messages.EnrollmentCreated,
            AppConstants.StatusCodes.Created);
    }

    public async Task<ApiResponse<EnrollmentDto>> UpdateAsync(int id, UpdateEnrollmentDto updateDto)
    {
        var existingEnrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (existingEnrollment == null)
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                AppConstants.Messages.EnrollmentNotFound,
                AppConstants.StatusCodes.NotFound);
        }

        // Validate status
        var validStatuses = new[] { "Active", "Inactive", "Completed" };
        if (!validStatuses.Contains(updateDto.Status))
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                "Invalid status value",
                AppConstants.StatusCodes.BadRequest);
        }

        // Check class capacity if changing to Active
        if (updateDto.Status == "Active" && existingEnrollment.Status != "Active")
        {
            if (!await _enrollmentRepository.CanEnrollAsync(existingEnrollment.ClassId))
            {
                return ApiResponse<EnrollmentDto>.ErrorResponse(
                    "Class has reached maximum capacity",
                    AppConstants.StatusCodes.BadRequest);
            }
        }

        _mapper.Map(updateDto, existingEnrollment);
        var updatedEnrollment = await _enrollmentRepository.UpdateAsync(existingEnrollment);
        var enrollmentDto = _mapper.Map<EnrollmentDto>(updatedEnrollment);

        return ApiResponse<EnrollmentDto>.SuccessResponse(
            enrollmentDto,
            AppConstants.Messages.EnrollmentUpdated);
    }

    public async Task<ApiResponse<EnrollmentDto>> DeleteAsync(int id)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                AppConstants.Messages.EnrollmentNotFound,
                AppConstants.StatusCodes.NotFound);
        }

        await _enrollmentRepository.DeleteAsync(id);
        var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);
        return ApiResponse<EnrollmentDto>.SuccessResponse(
            enrollmentDto,
            AppConstants.Messages.EnrollmentDeleted);
    }
}