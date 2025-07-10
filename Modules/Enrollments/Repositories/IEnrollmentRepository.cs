using SchoolManagementSystem.Modules.Enrollments.Entities;
using SchoolManagementSystem.Common.Requests;

namespace SchoolManagementSystem.Modules.Enrollments.Repositories;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id);
    Task<(List<Enrollment> enrollments, int totalCount)> GetAllAsync(PaginationRequest request);
    Task<(List<Enrollment> enrollments, int totalCount)> GetByStudentIdAsync(int studentId, PaginationRequest request);
    Task<(List<Enrollment> enrollments, int totalCount)> GetByClassIdAsync(int classId, PaginationRequest request);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<Enrollment> PatchAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> DuplicateEnrollmentExistsAsync(int studentId, int classId);
    Task<bool> StudentExistsAsync(int studentId);
    Task<bool> ClassExistsAsync(int classId);
    Task<bool> CanEnrollAsync(int classId);
    Task<bool> IsClassOwnedByTeacherAsync(int classId, int teacherId);
}