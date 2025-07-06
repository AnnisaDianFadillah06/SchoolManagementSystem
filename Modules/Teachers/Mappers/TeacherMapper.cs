using AutoMapper;
using SchoolManagementSystem.Modules.Teachers.Entities; 
using SchoolManagementSystem.Modules.Teachers.Dtos;

namespace SchoolManagementSystem.Modules.Teachers.Mappers
{
    public class TeacherMapper : Profile
    {
        public TeacherMapper()
        {
            CreateMap<Teacher, TeacherDto>();

            CreateMap<CreateTeacherDto, Teacher>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateTeacherDto, Teacher>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}