using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BrewLab.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = GetProjectPath();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connStr = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                throw new InvalidOperationException($"Connection string 'DefaultConnection' not found. Looked in: {basePath}");
            }

            optionsBuilder.UseNpgsql(connStr);
            return new AppDbContext(optionsBuilder.Options);
        }

        private static string GetProjectPath()
        {
            // Search upward from the current directory for appsettings.json (design-time runs from different cwd)
            var dir = Directory.GetCurrentDirectory();
            for (var i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
            {
                if (File.Exists(Path.Combine(dir, "appsettings.json")))
                {
                    return dir;
                }

                var parent = Directory.GetParent(dir);
                dir = parent?.FullName;
            }

            // Fallback to assembly location / BaseDirectory
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        }
    }
}
