using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Students.Dtos;

namespace SchoolManagementSystem.Modules.Enrollments.Dtos
{

    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Inactive, Completed public StudentDto? Student { get; set; } public ClassDto? Class { get; set; } }
        public StudentDto Student { get; set; } = null!;
        public ClassDto Class { get; set; } = null!;
    }
    public class CreateEnrollmentDto
    {
        [Required] public int StudentId { get; set; }

        [Required]
        public int ClassId { get; set; }

    }

    public class PatchEnrollmentDto
    {
        public int? StudentId { get; set; }

        public int? ClassId { get; set; }

        [Required]
        [RegularExpression("Active|Inactive|Completed", ErrorMessage = "Status must be Active, Inactive, or Completed")]
        public string Status { get; set; } = string.Empty;
    }

}