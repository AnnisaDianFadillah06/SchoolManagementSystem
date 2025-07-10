using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Common.Responses;

namespace SchoolManagementSystem.Modules.Classes.Services;

public interface IClassService
{
    Task<ApiResponse<PaginatedResponse<ClassDto>>> GetAllAsync(PaginationRequest request);
    Task<ApiResponse<ClassDto>> CreateAsync(CreateClassDto createDto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<ClassDto>> GetByIdAsync(int id, int? teacherId, int? studentId, string userRole);
    Task<ApiResponse<ClassDto>> PatchAsync(int id, PatchClassDto patchDto, int? teacherId, string userRole);

}