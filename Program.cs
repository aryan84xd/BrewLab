using System.Text;
using System.Text;
using BrewLab.Data;
using BrewLab.Options;
using BrewLab.Repositories;
using BrewLab.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

// Bind to all interfaces (0.0.0.0) in production, localhost in development
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls($"http://localhost:{port}");
}
else
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}


// Bind Jwt settings
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() 
    ?? throw new InvalidOperationException("JWT settings not configured");

// Validate JWT Key is configured
if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException(
        "JWT Key is not configured. Please set Jwt:Key in appsettings.json or environment variable Jwt__Key");
}

if (jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Key must be at least 32 characters long for security purposes");
}

builder.Services.AddSingleton(jwtSettings);

// Configure PostgreSQL database and repositories for production
// Expect the connection string to be provided via configuration (e.g. environment
// variable 'ConnectionStrings__DefaultConnection'). The NpgsqlConnectionFactory
// will read that connection string at runtime.
builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

// PostgreSQL Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICoffeeRepository, CoffeeRepository>();
builder.Services.AddScoped<IExperimentRepository, ExperimentRepository>();

// Services (work with both database types)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICoffeeService, CoffeeService>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:4173",
                "http://localhost:5000",
                "http://127.0.0.1:5000",
                "https://brew-lab-frontend.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    // Development policy - allow everything
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JWT Authentication
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // SaveToken makes the raw token available via AuthenticationProperties if needed
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero,
            // Ensure the NameClaimType maps to the 'sub' claim so ClaimTypes.NameIdentifier/"sub" are populated
            NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub
        };

        // Add simple logging hooks to help diagnose authentication problems at runtime
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()
                             ?.CreateLogger("JwtAuth");
                logger?.LogError(ctx.Exception, "JWT authentication failed");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()
                             ?.CreateLogger("JwtAuth");
                var sub = ctx.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                          ?? ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                logger?.LogInformation("JWT token validated for subject {sub}", sub);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Swagger + JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter 'Bearer {token}'",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Use appropriate CORS policy based on environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll"); // Allow all origins in development
}
else
{
    app.UseCors("AllowFrontend"); // Restricted CORS in production
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();