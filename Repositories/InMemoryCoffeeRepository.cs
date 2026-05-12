using BrewLab.Data;
using BrewLab.Models.DBO;

namespace BrewLab.Repositories
{
    public class InMemoryCoffeeRepository : ICoffeeRepository
    {
        private readonly IInMemoryDatabase _db;

        public InMemoryCoffeeRepository(IInMemoryDatabase db)
        {
            _db = db;
        }

        public Task<CoffeeDBO?> GetByIdAsync(Guid id, Guid userId)
        {
            _db.Coffees.TryGetValue(id, out var coffee);
            if (coffee != null && coffee.UserId == userId)
            {
                return Task.FromResult<CoffeeDBO?>(coffee);
            }
            return Task.FromResult<CoffeeDBO?>(null);
        }

        public Task<IEnumerable<CoffeeDBO>> GetAllByUserIdAsync(Guid userId)
        {
            var coffees = _db.Coffees.Values.Where(c => c.UserId == userId).ToList();
            return Task.FromResult<IEnumerable<CoffeeDBO>>(coffees);
        }

        public Task<CoffeeDBO> CreateAsync(CoffeeDBO coffee)
        {
            coffee.Id = Guid.NewGuid();
            _db.Coffees.TryAdd(coffee.Id, coffee);
            return Task.FromResult(coffee);
        }

        public Task<bool> ExistsAsync(Guid id, Guid userId)
        {
            var exists = _db.Coffees.TryGetValue(id, out var coffee) && coffee.UserId == userId;
            return Task.FromResult(exists);
        }
    }
}
