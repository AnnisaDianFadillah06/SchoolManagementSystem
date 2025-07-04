using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.Configurations
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}