using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Enrollments.Entities;
using SchoolManagementSystem.Common.Requests; 
using SchoolManagementSystem.Configurations;

namespace SchoolManagementSystem.Modules.Enrollments.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {

        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Class)
                .ThenInclude(c => c.Teacher)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<(List<Enrollment> enrollments, int totalCount)> GetAllAsync(PaginationRequest request)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Class)
                .ThenInclude(c => c.Teacher)
                .AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(e => e.Student.FirstName.Contains(request.Search) ||
                                        e.Student.LastName.Contains(request.Search) ||
                                        e.Class.ClassName.Contains(request.Search) ||
                                        e.Class.Teacher.Subject.Contains(request.Search));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "studentname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Student.FirstName)
                            : query.OrderBy(e => e.Student.FirstName);
                        break;
                    case "classname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Class.ClassName)
                            : query.OrderBy(e => e.Class.ClassName);
                        break;
                    case "enrollmentdate":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.EnrollmentDate)
                            : query.OrderBy(e => e.EnrollmentDate);
                        break;
                    case "status":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Status)
                            : query.OrderBy(e => e.Status);
                        break;
                    default:
                        query = query.OrderBy(e => e.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            // Pagination
            var enrollments = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (enrollments, totalCount);
        }

        public async Task<(List<Enrollment> enrollments, int totalCount)> GetByStudentIdAsync(int studentId, PaginationRequest request)
        {
            var query = _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Student)
                .Include(e => e.Class)
                .ThenInclude(c => c.Teacher)
                .AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(e => e.Student.FirstName.Contains(request.Search) ||
                                        e.Student.LastName.Contains(request.Search) ||
                                        e.Class.ClassName.Contains(request.Search) ||
                                        e.Class.Teacher.Subject.Contains(request.Search));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "studentname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Student.FirstName)
                            : query.OrderBy(e => e.Student.FirstName);
                        break;
                    case "classname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Class.ClassName)
                            : query.OrderBy(e => e.Class.ClassName);
                        break;
                    case "enrollmentdate":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.EnrollmentDate)
                            : query.OrderBy(e => e.EnrollmentDate);
                        break;
                    case "status":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Status)
                            : query.OrderBy(e => e.Status);
                        break;
                    default:
                        query = query.OrderBy(e => e.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            // Pagination
            var enrollments = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (enrollments, totalCount);
        }

        public async Task<(List<Enrollment> enrollments, int totalCount)> GetByClassIdAsync(int classId, PaginationRequest request)
        {
            var query = _context.Enrollments
                .Where(e => e.ClassId == classId)
                .Include(e => e.Student)
                .Include(e => e.Class)
                .ThenInclude(c => c.Teacher)
                .AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(e => e.Student.FirstName.Contains(request.Search) ||
                                        e.Student.LastName.Contains(request.Search) ||
                                        e.Class.ClassName.Contains(request.Search) ||
                                        e.Class.Teacher.Subject.Contains(request.Search));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "studentname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Student.FirstName)
                            : query.OrderBy(e => e.Student.FirstName);
                        break;
                    case "classname":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Class.ClassName)
                            : query.OrderBy(e => e.Class.ClassName);
                        break;
                    case "enrollmentdate":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.EnrollmentDate)
                            : query.OrderBy(e => e.EnrollmentDate);
                        break;
                    case "status":
                        query = request.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Status)
                            : query.OrderBy(e => e.Status);
                        break;
                    default:
                        query = query.OrderBy(e => e.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            // Pagination
            var enrollments = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (enrollments, totalCount);
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Enrollments.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> DuplicateEnrollmentExistsAsync(int studentId, int classId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.ClassId == classId && e.Status == "Active");
        }

        public async Task<bool> StudentExistsAsync(int studentId)
        {
            return await _context.Students.AnyAsync(s => s.Id == studentId);
        }

        public async Task<bool> ClassExistsAsync(int classId)
        {
            return await _context.Classes.AnyAsync(c => c.Id == classId);
        }

        public async Task<bool> CanEnrollAsync(int classId)
        {
            var classEntity = await _context.Classes
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classEntity == null)
                return false;

            return classEntity.Enrollments.Count(e => e.Status == "Active") < classEntity.MaxStudents;
        }
    }
}

