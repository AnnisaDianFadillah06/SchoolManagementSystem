using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Common.Responses;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public interface IStudentService
    {
        Task<ApiResponse<StudentDto>> GetByIdAsync(int id);
        Task<ApiResponse<PaginatedResponse<StudentDto>>> GetAllAsync(PaginationRequest request);
        Task<ApiResponse<StudentDto>> CreateAsync(CreateStudentDto createDto);
        Task<ApiResponse<StudentDto>> PatchAsync(int id, PatchStudentDto patchDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}