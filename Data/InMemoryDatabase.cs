using BrewLab.Models.DBO;
using System.Collections.Concurrent;

namespace BrewLab.Data
{
    public interface IInMemoryDatabase
    {
        // Users
        ConcurrentDictionary<Guid, UserDBO> Users { get; }

        // Coffees
        ConcurrentDictionary<Guid, CoffeeDBO> Coffees { get; }

        // Experiments
        ConcurrentDictionary<Guid, ExperimentDBO> Experiments { get; }

        void Seed();
    }

    public class InMemoryDatabase : IInMemoryDatabase
    {
        public ConcurrentDictionary<Guid, UserDBO> Users { get; } = new();
        public ConcurrentDictionary<Guid, CoffeeDBO> Coffees { get; } = new();
        public ConcurrentDictionary<Guid, ExperimentDBO> Experiments { get; } = new();

        public InMemoryDatabase()
        {
            Seed();
        }

        public void Seed()
        {
            // Seed a test user
            var testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            Users.TryAdd(testUserId, new UserDBO
            {
                Id = testUserId,
                Name = "Test User",
                Email = "test@brewlab.com",
                // Password: "Test123!" - BCrypt hash
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
            });

            // Seed some test coffees
            var coffee1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            Coffees.TryAdd(coffee1Id, new CoffeeDBO
            {
                Id = coffee1Id,
                Name = "Ethiopian Yirgacheffe",
                Brand = "Blue Bottle",
                Roast = "Light",
                Origin = "Ethiopia",
                TastingNotes = "Floral, citrus, tea-like",
                UserId = testUserId
            });

            var coffee2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            Coffees.TryAdd(coffee2Id, new CoffeeDBO
            {
                Id = coffee2Id,
                Name = "Colombian Supremo",
                Brand = "Stumptown",
                Roast = "Medium",
                Origin = "Colombia",
                TastingNotes = "Chocolate, caramel, nutty",
                UserId = testUserId
            });

            // Seed an experiment
            var experiment1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            Experiments.TryAdd(experiment1Id, new ExperimentDBO
            {
                Id = experiment1Id,
                CoffeeId = coffee1Id,
                UserId = testUserId,
                Date = DateTime.UtcNow.AddDays(-1),
                BrewMethod = "V60",
                CoffeeWeight = 18.5m,
                WaterWeight = 300m,
                BrewTime = new TimeOnly(0, 2, 30),
                Remark = "Perfect extraction, bright acidity",
                Aroma = 5,
                Acidity = 4,
                Body = 3,
                Overall = 9
            });
        }
    }
}
