using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Common.Requests;
using SchoolManagementSystem.Configurations;

namespace SchoolManagementSystem.Modules.Students.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<(List<Student> students, int totalCount)> GetAllAsync(PaginationRequest request)
        {
            var query = _context.Students.AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(s => s.FirstName.Contains(request.Search) || 
                                       s.LastName.Contains(request.Search) || 
                                       s.Email.Contains(request.Search));
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
                            ? query.OrderByDescending(s => s.FirstName)
                            : query.OrderBy(s => s.FirstName);
                        break;
                    case "lastname":
                        query = request.SortOrder?.ToLower() == "desc" 
                            ? query.OrderByDescending(s => s.LastName)
                            : query.OrderBy(s => s.LastName);
                        break;
                    case "email":
                        query = request.SortOrder?.ToLower() == "desc" 
                            ? query.OrderByDescending(s => s.Email)
                            : query.OrderBy(s => s.Email);
                        break;
                    case "createdat":
                        query = request.SortOrder?.ToLower() == "desc" 
                            ? query.OrderByDescending(s => s.CreatedAt)
                            : query.OrderBy(s => s.CreatedAt);
                        break;
                    default:
                        query = query.OrderBy(s => s.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.Id);
            }

            // Pagination
            var students = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (students, totalCount);
        }

        public async Task<Student> CreateAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> PatchAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Students.Where(s => s.Email == email);
            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);
            
            return await query.AnyAsync();
        }
    }
}