using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Configurations;

namespace SchoolManagementSystem.Modules.Teachers.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;
        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }

        public async Task<(List<Teacher> teachers, int totalCount)> GetAllAsync(PaginationRequest request)
        {
            var query = _context.Teachers.AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(t => t.FirstName.Contains(request.Search) ||
                                    t.LastName.Contains(request.Search) ||
                                    t.Email.Contains(request.Search) ||
                                    t.NIP.Contains(request.Search));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "firstname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.FirstName)
                            : query.OrderBy(t => t.FirstName);
                        break;
                    case "lastname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.LastName)
                            : query.OrderBy(t => t.LastName);
                        break;
                    case "email":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.Email)
                            : query.OrderBy(t => t.Email);
                        break;
                    case "nip":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.NIP)
                            : query.OrderBy(t => t.NIP);
                        break;
                    case "createdat":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.CreatedAt)
                            : query.OrderBy(t => t.CreatedAt);
                        break;
                    default:
                        query = query.OrderBy(t => t.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(t => t.Id);
            }

            // Pagination
            var teachers = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (teachers, totalCount);
        }

        public async Task<Teacher> CreateAsync(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<Teacher> UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return false;

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teachers.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Teachers.Where(t => t.Email == email);
            if (excludeId.HasValue)
                query = query.Where(t => t.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> NIPExistsAsync(string nip, int? excludeId = null)
        {
            var query = _context.Teachers.Where(t => t.NIP == nip);
            if (excludeId.HasValue)
                query = query.Where(t => t.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}