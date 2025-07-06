using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Common.Responses;

namespace SchoolManagementSystem.Modules.Classes.Services;

public interface IClassService
{
    Task<ApiResponse<ClassDto>> GetByIdAsync(int id);
    Task<ApiResponse<PaginatedResponse<ClassDto>>> GetAllAsync(PaginationRequest request);
    Task<ApiResponse<ClassDto>> CreateAsync(CreateClassDto createDto);
    Task<ApiResponse<ClassDto>> UpdateAsync(int id, UpdateClassDto updateDto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}