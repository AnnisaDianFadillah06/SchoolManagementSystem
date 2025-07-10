using AutoMapper;
using SchoolManagementSystem.Modules.Enrollments.Services;
using SchoolManagementSystem.Modules.Enrollments.Repositories;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Entities;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;
using SchoolManagementSystem.Common.Constants;
using System.Collections.Generic;

namespace SchoolManagementSystem.Modules.Enrollments.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;
    private readonly IClassRepository _classRepository;
    private readonly IStudentRepository _studentRepository;


    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper,
        IClassRepository classRepository,
        IStudentRepository studentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
        _classRepository = classRepository;
        _studentRepository = studentRepository;
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

        return ApiResponse<PaginatedResponse<EnrollmentDto>>.SuccessResponse(
            paginatedResponse,
            AppConstants.Messages.EnrollmentRetrieved);
    }

    public async Task<ApiResponse<PaginatedResponse<EnrollmentDto>>> GetByStudentIdAsync(int studentId, PaginationRequest request)
    {
        if (!await _enrollmentRepository.StudentExistsAsync(studentId))
        {
            return ApiResponse<PaginatedResponse<EnrollmentDto>>.ErrorResponse(
                "Student not found",
                AppConstants.StatusCodes.NotFound);
        }

        var (enrollments, totalCount) = await _enrollmentRepository.GetByStudentIdAsync(studentId, request);
        var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);

        var paginatedResponse = PaginatedResponse<EnrollmentDto>.Create(
            enrollmentDtos, totalCount, request.Page, request.PageSize);

        return ApiResponse<PaginatedResponse<EnrollmentDto>>.SuccessResponse(
            paginatedResponse,
            AppConstants.Messages.EnrollmentRetrieved);
    }

    public async Task<ApiResponse<PaginatedResponse<EnrollmentDto>>> GetByClassIdAsync(int classId, PaginationRequest request)
    {
        if (!await _enrollmentRepository.ClassExistsAsync(classId))
        {
            return ApiResponse<PaginatedResponse<EnrollmentDto>>.ErrorResponse(
                "Class not found",
                AppConstants.StatusCodes.NotFound);
        }

        var (enrollments, totalCount) = await _enrollmentRepository.GetByClassIdAsync(classId, request);
        var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);

        var paginatedResponse = PaginatedResponse<EnrollmentDto>.Create(
            enrollmentDtos, totalCount, request.Page, request.PageSize);

        return ApiResponse<PaginatedResponse<EnrollmentDto>>.SuccessResponse(
            paginatedResponse,
            AppConstants.Messages.EnrollmentRetrieved);
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
                new List<string> { "Duplicate Enrollment" });
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

    public async Task<ApiResponse<EnrollmentDto>> PatchAsync(int id, PatchEnrollmentDto patchDto)
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
        if (!validStatuses.Contains(patchDto.Status))
        {
            return ApiResponse<EnrollmentDto>.ErrorResponse(
                "Invalid status value",
                AppConstants.StatusCodes.BadRequest);
        }

        bool isChangingClass = patchDto.ClassId.HasValue && patchDto.ClassId != existingEnrollment.ClassId;
        bool isChangingStudent = patchDto.StudentId.HasValue && patchDto.StudentId != existingEnrollment.StudentId;

        if (isChangingClass)
        {
            var newClassId = patchDto.ClassId.Value;
            if (!await _classRepository.ExistsAsync(newClassId))
            {
                return ApiResponse<EnrollmentDto>.ErrorResponse(
                    "Class not found",
                    AppConstants.StatusCodes.BadRequest);
            }

            if (patchDto.Status == "Active")
            {
                if (!await _enrollmentRepository.CanEnrollAsync(newClassId))
                {
                    return ApiResponse<EnrollmentDto>.ErrorResponse(
                        "Class has reached maximum capacity",
                        AppConstants.StatusCodes.BadRequest);
                }
            }

            existingEnrollment.ClassId = newClassId;
        }

        if (isChangingStudent)
        {
            var newStudentId = patchDto.StudentId.Value;
            if (!await _studentRepository.ExistsAsync(newStudentId))
            {
                return ApiResponse<EnrollmentDto>.ErrorResponse(
                    "Student not found",
                    AppConstants.StatusCodes.BadRequest);
            }

            existingEnrollment.StudentId = newStudentId;
        }

        existingEnrollment.Status = patchDto.Status;
        existingEnrollment.UpdatedAt = DateTime.UtcNow;

        var updated = await _enrollmentRepository.PatchAsync(existingEnrollment);
        var dto = _mapper.Map<EnrollmentDto>(updated);

        return ApiResponse<EnrollmentDto>.SuccessResponse(
            dto,
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

    public async Task<bool> IsClassOwnedByTeacherAsync(int classId, int teacherId)
    {
        return await _enrollmentRepository.IsClassOwnedByTeacherAsync(classId, teacherId);
    }

}