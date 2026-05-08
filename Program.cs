using System.Text;
using BrewLab.Abstraction.Data;
using BrewLab.Abstraction.Repositories;
using BrewLab.Abstraction.Services;
using BrewLab.BusinessLogic.Services;
using BrewLab.Common.Options;
using BrewLab.DataAccess.Data;
using BrewLab.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}", $"http://[::]:{port}");

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

// Database connection factory
builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICoffeeRepository, CoffeeRepository>();
builder.Services.AddScoped<IExperimentRepository, ExperimentRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICoffeeService, CoffeeService>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(BrewLab.ApiServices.Controllers.AuthController).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:4173",
                "https://brew-lab-frontend.vercel.app"
            )
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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();