// Modules/Users/Repositories/IUserRepository.cs
using SchoolManagementSystem.Modules.Users.Entities;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Modules.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string username, string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}