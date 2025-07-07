using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace SchoolManagementSystem.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[^1];

            if (token != null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtSecret = _configuration["JwtSettings:Secret"] ?? Environment.GetEnvironmentVariable("JwtSettings__Secret");
                    if (string.IsNullOrEmpty(jwtSecret))
                    {
                        throw new InvalidOperationException("JWT Secret is not configured.");
                    }
                    var key = Encoding.ASCII.GetBytes(jwtSecret);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JwtSettings__Issuer"),
                        ValidateAudience = true,
                        ValidAudience = _configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("JwtSettings__Audience"),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    context.Items["UserId"] = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
                    context.Items["Role"] = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                    context.Items["StudentId"] = jwtToken.Claims.FirstOrDefault(x => x.Type == "StudentId")?.Value;
                    context.Items["TeacherId"] = jwtToken.Claims.FirstOrDefault(x => x.Type == "TeacherId")?.Value;
                }
                catch
                {
                    // Token tidak valid, lanjutkan tanpa user context
                }
            }

            await _next(context);
        }
    }
}