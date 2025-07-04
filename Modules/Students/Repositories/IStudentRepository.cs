using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Common.Requests;

namespace SchoolManagementSystem.Modules.Students.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id);
        Task<(List<Student> students, int totalCount)> GetAllAsync(PaginationRequest request);
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}