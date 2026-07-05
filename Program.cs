using BrewLab.Authentication;
using BrewLab.Data;
using BrewLab.Repositories.Implementations;
using BrewLab.Repositories.Interfaces;
using BrewLab.Services.Implementations;
using BrewLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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


//Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Database connection string not configured");
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();

builder.Services.AddJwtAuthentication(builder.Configuration);

// Register Authentication Services
builder.Services.AddScoped<ResponseFactory>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Coffee Services
builder.Services.AddScoped<ICoffeeRepository, CoffeeRepository>();
builder.Services.AddScoped<ICoffeeService, CoffeeService>();
builder.Services.AddScoped<IGrinderService, GrinderService>();
builder.Services.AddScoped<IBrewerService, BrewerService>();
//Register Experiment Services
builder.Services.AddScoped<IExperimentRepository, ExperimentRepository>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();
builder.Services.AddScoped<IGrinderRepository, GrinderRepository>();
builder.Services.AddScoped<IBrewerRepository, BrewerRepository>();

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

   
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddOpenApi();

var app = builder.Build();

// Use appropriate CORS policy based on environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("BrewLab API")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    app.UseCors("AllowAll");
}
else
{
    app.UseCors("AllowFrontend");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();