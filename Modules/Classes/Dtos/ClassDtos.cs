using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Modules.Classes.Dtos
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public int CurrentStudentCount { get; set; }
        public string? Schedule { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateClassDto
    {
        [Required]
        [StringLength(50)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        public int TeacherId { get; set; }

        [Range(1, 100)]
        public int MaxStudents { get; set; } = 30;

        [StringLength(200)]
        public string? Schedule { get; set; }

    }

    public class UpdateClassDto
    {
        [Required]
        [StringLength(50)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        public int TeacherId { get; set; }

        [Range(1, 100)]
        public int MaxStudents { get; set; } = 30;

        [StringLength(200)]
        public string? Schedule { get; set; }

    }
}