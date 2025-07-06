using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Common.Requests;

namespace SchoolManagementSystem.Modules.Teachers.Repositories
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetByIdAsync(int id);
        Task<(List<Teacher> teachers, int totalCount)> GetAllAsync(PaginationRequest request);
        Task<Teacher> CreateAsync(Teacher teacher);
        Task<Teacher> UpdateAsync(Teacher teacher);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<bool> NIPExistsAsync(string nip, int? excludeId = null); }
}
