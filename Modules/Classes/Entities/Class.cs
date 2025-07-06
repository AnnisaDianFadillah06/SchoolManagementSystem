using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Enrollments.Entities;

namespace SchoolManagementSystem.Modules.Classes.Entities
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ClassName { get; set; } = string.Empty; // e.g., "10A", "11B"
        
        [Required]
        [StringLength(100)]
        public string Subject { get; set; } = string.Empty; // Mata Pelajaran
        
        [Required]
        public int TeacherId { get; set; }
        
        [Range(1, 100)]
        public int MaxStudents { get; set; } = 30; // Kapasitas maksimal
        
        [StringLength(200)]
        public string? Schedule { get; set; } // Jadwal kelas
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; } = null!;
        
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        
        // Computed property
        [NotMapped]
        public int CurrentStudentCount => Enrollments?.Count(e => e.Status == "Active") ?? 0;
    }
}