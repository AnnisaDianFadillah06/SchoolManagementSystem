using AutoMapper;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Students.Dtos;

namespace SchoolManagementSystem.Modules.Students.Mappers
{
    public class StudentMapper : Profile
    {
        public StudentMapper()
        {
            CreateMap<Student, StudentDto>();
            
            CreateMap<CreateStudentDto, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore ID - let database generate it
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
                
            CreateMap<PatchStudentDto, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Don't map ID for updates
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Don't update CreatedAt
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}