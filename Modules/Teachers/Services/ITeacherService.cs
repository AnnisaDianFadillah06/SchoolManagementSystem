using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Common.Responses;

namespace SchoolManagementSystem.Modules.Teachers.Services
{
    public interface ITeacherService {
        Task<ApiResponse<TeacherDto>> GetByIdAsync(int id);
        Task<ApiResponse<PaginatedResponse<TeacherDto>>> GetAllAsync(PaginationRequest request);
        Task<ApiResponse<TeacherDto>> CreateAsync(CreateTeacherDto createDto);
        Task<ApiResponse<TeacherDto>> PatchAsync(int id, PatchTeacherDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(int id); }
}