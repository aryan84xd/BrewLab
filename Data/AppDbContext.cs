using BrewLab.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Coffee> Coffees => Set<Coffee>();
    public DbSet<Experiment> Experiments => Set<Experiment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User → Coffee (1:N)
        modelBuilder.Entity<Coffee>()
            .HasOne(c => c.User)
            .WithMany(u => u.Coffees)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Coffee → Experiment (1:N)
        modelBuilder.Entity<Experiment>()
            .HasOne(e => e.Coffee)
            .WithMany(c => c.Experiments)
            .HasForeignKey(e => e.CoffeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}