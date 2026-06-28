using BrewLab.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrewLab.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Coffee> Coffees => Set<Coffee>();
        public DbSet<Experiment> Experiments => Set<Experiment>();
        public DbSet<BrewMethod> BrewMethods => Set<BrewMethod>();
        public DbSet<BrewParameter> BrewParameters => Set<BrewParameter>();
        public DbSet<ExperimentParameter> ExperimentParameters => Set<ExperimentParameter>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Coffee
            modelBuilder.Entity<Coffee>()
                .HasOne(c => c.User)
                .WithMany(u => u.Coffees)
                .HasForeignKey(c => c.UserId);

            // Experiment
            modelBuilder.Entity<Experiment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Experiments)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Experiment>()
                .HasOne(e => e.Coffee)
                .WithMany(c => c.Experiments)
                .HasForeignKey(e => e.CoffeeId);

            modelBuilder.Entity<Experiment>()
                .HasOne(e => e.BrewMethod)
                .WithMany(b => b.Experiments)
                .HasForeignKey(e => e.BrewMethodId);

            // BrewParameter
            modelBuilder.Entity<BrewParameter>()
                .HasOne(bp => bp.BrewMethod)
                .WithMany(bm => bm.BrewParameters)
                .HasForeignKey(bp => bp.BrewMethodId);

            // ExperimentParameter
            modelBuilder.Entity<ExperimentParameter>()
                .HasOne(ep => ep.Experiment)
                .WithMany(e => e.Parameters)
                .HasForeignKey(ep => ep.ExperimentId);

            modelBuilder.Entity<ExperimentParameter>()
                .HasOne(ep => ep.BrewParameter)
                .WithMany(bp => bp.ExperimentParameters)
                .HasForeignKey(ep => ep.BrewParameterId);
        }
    }
}