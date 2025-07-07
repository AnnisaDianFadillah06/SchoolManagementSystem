using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Common.Responses;

namespace SchoolManagementSystem.Modules.Enrollments.Services
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<EnrollmentDto>> GetByIdAsync(int id);
        Task<ApiResponse<PaginatedResponse<EnrollmentDto>>> GetAllAsync(PaginationRequest request);
        Task<ApiResponse<EnrollmentDto>> CreateAsync(CreateEnrollmentDto createDto);
        Task<ApiResponse<EnrollmentDto>> UpdateAsync(int id, UpdateEnrollmentDto updateDto);
        Task<ApiResponse<EnrollmentDto>> DeleteAsync(int id);
        Task<ApiResponse<PaginatedResponse<EnrollmentDto>>> GetByStudentIdAsync(int studentId, PaginationRequest request);
        Task<ApiResponse<PaginatedResponse<EnrollmentDto>>> GetByClassIdAsync(int classId, PaginationRequest request);
        Task<bool> IsClassOwnedByTeacherAsync(int classId, int teacherId);
    }
}