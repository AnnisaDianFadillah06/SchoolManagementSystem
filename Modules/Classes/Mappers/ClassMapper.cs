using AutoMapper;
using SchoolManagementSystem.Modules.Classes.Entities; 
using SchoolManagementSystem.Modules.Classes.Dtos;

namespace SchoolManagementSystem.Modules.Classes.Mappers;

public class ClassMapper : Profile 
{
    public ClassMapper()
    {
        CreateMap<Class, ClassDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Teacher.Subject))
            .ForMember(dest => dest.CurrentStudentCount, opt => opt.MapFrom(src => src.CurrentStudentCount));

        CreateMap<CreateClassDto, Class>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateClassDto, Class>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }

}