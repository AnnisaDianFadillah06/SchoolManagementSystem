using Microsoft.Extensions.DependencyInjection;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Services;
using SchoolManagementSystem.Modules.Teachers.Repositories;
using SchoolManagementSystem.Modules.Teachers.Services;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Services;
using SchoolManagementSystem.Modules.Classes.Repositorie;

namespace SchoolManagementSystem.Configurations
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(SchoolManagementSystem.Modules.Students.Mappers.StudentMapper));

            // Register Student Services
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentService>();

            // TODO: Register other services (Teacher, Class, Enrollment, etc.)
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();

            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IClassRepository, ClassRepository>();

            return services;
        }
    }
}