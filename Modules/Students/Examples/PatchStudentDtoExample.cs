using Swashbuckle.AspNetCore.Filters;
using SchoolManagementSystem.Modules.Students.Dtos;

namespace SchoolManagementSystem.Modules.Students.Examples;
public class PatchStudentDtoExample : IExamplesProvider<PatchStudentDto>
{
    public PatchStudentDto GetExamples()
    {
        return new PatchStudentDto
        {
            NISN = "1234567890",
            FirstName = "Budi",
            LastName = "Santoso",
            Email = "budi@example.com",
            Phone = "08123456789",
            DateOfBirth = new DateTime(2005, 5, 20),
            Address = "Jl. Merdeka No.123, Bandung"
        };
    }
}
