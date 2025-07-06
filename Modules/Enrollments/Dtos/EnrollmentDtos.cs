using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Modules.Enrollments.Dtos
{

    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Inactive, Completed public StudentDto? Student { get; set; } public ClassDto? Class { get; set; } }
    }
    public class CreateEnrollmentDto
    {
        [Required] public int StudentId { get; set; }

        [Required]
        public int ClassId { get; set; }

    }

    public class UpdateEnrollmentDto
    {
        [Required] public string Status { get; set; } = string.Empty; // Active, Inactive, Completed 
    }
}