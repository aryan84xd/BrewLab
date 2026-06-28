using BrewLab.Data;
using BrewLab.Models.Entities;
using BrewLab.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewLab.Repositories.Implementations
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly AppDbContext _context;

        public CoffeeRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Coffee>> GetAllByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return Enumerable.Empty<Coffee>();

            return await _context.Coffees
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Coffee?> GetByIdAsync(Guid coffeeId)
        {
            if (coffeeId == Guid.Empty)
                return null;

            return await _context.Coffees
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == coffeeId);
        }

        public async Task<Coffee> CreateAsync(Coffee coffee)
        {
            if (coffee == null)
                throw new ArgumentNullException(nameof(coffee));

            _context.Coffees.Add(coffee);
            await _context.SaveChangesAsync();
            return coffee;
        }

        public async Task<Coffee> UpdateAsync(Coffee coffee)
        {
            if (coffee == null)
                throw new ArgumentNullException(nameof(coffee));

            _context.Coffees.Update(coffee);
            await _context.SaveChangesAsync();
            return coffee;
        }

        public async Task<bool> ExistsAsync(Guid coffeeId, Guid userId)
        {
            if (coffeeId == Guid.Empty || userId == Guid.Empty)
                return false;

            return await _context.Coffees
                .AnyAsync(c => c.Id == coffeeId && c.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
