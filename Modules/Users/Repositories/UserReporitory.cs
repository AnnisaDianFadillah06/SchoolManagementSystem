using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Configurations;
using SchoolManagementSystem.Modules.Users.Entities;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Modules.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            return await _context.Users.AnyAsync(u => 
                (!string.IsNullOrEmpty(username) && u.Username == username) || 
                (!string.IsNullOrEmpty(email) && u.Email == email));
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> StudentIdExistsAsync(int studentId)
        {
            return await _context.Users.AnyAsync(u => u.StudentId == studentId);
        }

        public async Task<bool> TeacherIdExistsAsync(int teacherId)
        {
            return await _context.Users.AnyAsync(u => u.TeacherId == teacherId);
        }
    }
}