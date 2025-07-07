using Microsoft.Extensions.DependencyInjection;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Services;
using SchoolManagementSystem.Modules.Teachers.Repositories;
using SchoolManagementSystem.Modules.Teachers.Services;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Services;
using SchoolManagementSystem.Modules.Classes.Repositorie;
using SchoolManagementSystem.Modules.Enrollments.Services;
using SchoolManagementSystem.Modules.Enrollments.Repositories;
using SchoolManagementSystem.Modules.Users.Services;
using SchoolManagementSystem.Modules.Users.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            // Register other services (Teacher, Class, Enrollment, etc.)
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();

            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IClassRepository, ClassRepository>();

            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

       public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSecret = configuration["JwtSettings:Secret"] ?? Environment.GetEnvironmentVariable("JwtSettings__Secret");
            var jwtIssuer = configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JwtSettings__Issuer");
            var jwtAudience = configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("JwtSettings__Audience");

            if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration (Secret, Issuer, or Audience) is missing.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                    };
                });

            return services;
        }
    }
}