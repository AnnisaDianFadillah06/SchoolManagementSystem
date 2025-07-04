using SchoolManagementSystem.Configurations;
using Microsoft.OpenApi.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load();

// Add services to the container
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "School Management System API", 
        Version = "v1",
        Description = "API untuk mengelola sistem manajemen sekolah"
    });
});

// Add Database
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Add Application Services
builder.Services.AddApplicationServices();

// Add CORS (optional, for frontend integration)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();