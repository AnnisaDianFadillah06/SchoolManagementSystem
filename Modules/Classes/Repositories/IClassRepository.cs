using SchoolManagementSystem.Modules.Classes.Entities; 
using SchoolManagementSystem.Common.Requests;

namespace SchoolManagementSystem.Modules.Classes.Repositories
{
   public interface IClassRepository
    {
        Task<Class?> GetByIdAsync(int id);
        Task<(List<Class> classes, int totalCount)> GetAllAsync(PaginationRequest request);
        Task<Class> CreateAsync(Class classEntity);
        Task<Class> UpdateAsync(Class classEntity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> TeacherExistsAsync(int teacherId);
        Task<bool> ClassNameExistsAsync(string className, int? excludeId = null); } 
}

