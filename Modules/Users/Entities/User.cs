using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Teachers.Entities;

namespace SchoolManagementSystem.Modules.Users.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }

        public bool IsActive { get; set; } = true;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Teacher? Teacher { get; set; }
    }
}