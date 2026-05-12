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

// Check if PostgreSQL is available
bool useInMemoryDb = false;
try
{
    var testConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(testConnectionString))
    {
        using var testConnection = new NpgsqlConnection(testConnectionString);
        testConnection.Open();
        testConnection.Close();
        Console.WriteLine("? PostgreSQL connection successful - Using PostgreSQL database");
    }
    else
    {
        useInMemoryDb = true;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"? PostgreSQL not available: {ex.Message}");
    Console.WriteLine("? Falling back to In-Memory database");
    useInMemoryDb = true;
}

if (useInMemoryDb)
{
    // In-Memory Database
    builder.Services.AddSingleton<IInMemoryDatabase, InMemoryDatabase>();

    // In-Memory Repositories
    builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
    builder.Services.AddScoped<ICoffeeRepository, InMemoryCoffeeRepository>();
    builder.Services.AddScoped<IExperimentRepository, InMemoryExperimentRepository>();
}
else
{
    // PostgreSQL Database
    builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

    // PostgreSQL Repositories
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ICoffeeRepository, CoffeeRepository>();
    builder.Services.AddScoped<IExperimentRepository, ExperimentRepository>();
}

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