using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Modules.Teachers.Dtos;

public class TeacherDto {
    public int Id { get; set; }
    public string NIP { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; } public string Subject { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; }
    public int TotalClasses { get; set; } }

public class CreateTeacherDto {
    [Required]
    [StringLength(20)]
    public string NIP { get; set; } = string.Empty;

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
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Qualification { get; set; } = string.Empty;

}

public class UpdateTeacherDto {
    [Required]
    [StringLength(20)]
    public string NIP { get; set; } = string.Empty;

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
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Qualification { get; set; } = string.Empty;

}