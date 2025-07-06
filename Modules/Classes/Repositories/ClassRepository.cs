using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Configurations;

namespace SchoolManagementSystem.Modules.Classes.Repositorie
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Class?> GetByIdAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(List<Class> classes, int totalCount)> GetAllAsync(PaginationRequest request)
        {
            var query = _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(c => c.ClassName.Contains(request.Search) ||
                                        c.Teacher.Subject.Contains(request.Search) ||
                                       c.Teacher.FirstName.Contains(request.Search) ||
                                       c.Teacher.LastName.Contains(request.Search));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "classname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(c => c.ClassName)
                            : query.OrderBy(c => c.ClassName);
                        break;
                    case "teacher":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(c => c.Teacher.FirstName)
                            : query.OrderBy(c => c.Teacher.FirstName);
                        break;
                    case "createdat":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(c => c.CreatedAt)
                            : query.OrderBy(c => c.CreatedAt);
                        break;
                    default:
                        query = query.OrderBy(c => c.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(c => c.Id);
            }

            // Pagination
            var classes = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (classes, totalCount);
        }

        public async Task<Class> CreateAsync(Class classEntity)
        {
            _context.Classes.Add(classEntity);
            await _context.SaveChangesAsync();
            return classEntity;
        }

        public async Task<Class> UpdateAsync(Class classEntity)
        {
            _context.Classes.Update(classEntity);
            await _context.SaveChangesAsync();
            return classEntity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
                return false;

            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Classes.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> TeacherExistsAsync(int teacherId)
        {
            return await _context.Teachers.AnyAsync(t => t.Id == teacherId);
        }

        public async Task<bool> ClassNameExistsAsync(string className, int? excludeId = null)
        {
            var query = _context.Classes.Where(c => c.ClassName == className);
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

    }
}