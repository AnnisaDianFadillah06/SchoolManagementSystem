using Microsoft.Extensions.DependencyInjection;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Services;

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

            return services;
        }
    }
}