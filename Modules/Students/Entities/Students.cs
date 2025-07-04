using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Modules.Students.Entities
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
    }
}