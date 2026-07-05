using BrewLab.Data;
using BrewLab.Models.Entities;
using BrewLab.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewLab.Repositories.Implementations
{
    public class GrinderRepository : IGrinderRepository
    {
        private readonly AppDbContext _context;

        public GrinderRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Grinder>> GetAllAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return Enumerable.Empty<Grinder>();

            return await _context.Grinders
                .AsNoTracking()
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<Grinder?> GetByIdAsync(Guid grinderId)
        {
            if (grinderId == Guid.Empty)
                return null;

            return await _context.Grinders
                .FirstOrDefaultAsync(g => g.Id == grinderId);
        }

        public async Task<Grinder> CreateAsync(Grinder grinder)
        {
            _context.Grinders.Add(grinder);
            await _context.SaveChangesAsync();
            return grinder;
        }

        public async Task<Grinder> UpdateAsync(Grinder grinder)
        {
            _context.Grinders.Update(grinder);
            await _context.SaveChangesAsync();
            return grinder;
        }

        public async Task DeleteAsync(Grinder grinder)
        {
            _context.Grinders.Remove(grinder);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid grinderId, Guid userId)
        {
            return await _context.Grinders
                .AnyAsync(g => g.Id == grinderId && g.UserId == userId);
        }
    }
}