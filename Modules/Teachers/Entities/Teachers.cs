using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Modules.Users.Entities;

namespace SchoolManagementSystem.Modules.Teachers.Entities
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string NIP { get; set; } = string.Empty; // Nomor Induk Pegawai
        
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
        
        [Required]
        [StringLength(100)]
        public string Subject { get; set; } = string.Empty; // Mata Pelajaran
        
        [StringLength(200)]
        public string? Qualification { get; set; } // Kualifikasi
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
        public virtual User? User { get; set; }
    }
}