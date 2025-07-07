using AutoMapper;
using SchoolManagementSystem.Modules.Users.Dtos;
using SchoolManagementSystem.Modules.Users.Entities;

namespace SchoolManagementSystem.Modules.Users.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
