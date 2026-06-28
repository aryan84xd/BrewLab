using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BrewLab.Authentication
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure JwtSettings from appsettings.json
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT configuration not found in appsettings.json");

            services.AddSingleton(jwtSettings);

            // Register JwtTokenGenerator
            services.AddSingleton<JwtTokenGenerator>();

            // Register IHttpContextAccessor for CurrentUserService
            services.AddHttpContextAccessor();

            // Register CurrentUserService
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Configure JWT Authentication
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Add Authorization
            services.AddAuthorization();

            return services;
        }
    }
}
