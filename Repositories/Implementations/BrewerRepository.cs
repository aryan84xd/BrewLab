using BrewLab.Data;
using BrewLab.Models.Entities;
using BrewLab.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewLab.Repositories.Implementations
{
    public class BrewerRepository : IBrewerRepository
    {
        private readonly AppDbContext _context;

        public BrewerRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Brewer>> GetAllAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return Enumerable.Empty<Brewer>();

            return await _context.Brewers
                .AsNoTracking()
                .Include(b => b.BrewMethod)
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Brewer?> GetByIdAsync(Guid brewerId)
        {
            if (brewerId == Guid.Empty)
                return null;

            return await _context.Brewers
                .Include(b => b.BrewMethod)
                .FirstOrDefaultAsync(b => b.Id == brewerId);
        }

        public async Task<Brewer> CreateAsync(Brewer brewer)
        {
            _context.Brewers.Add(brewer);
            await _context.SaveChangesAsync();
            return brewer;
        }

        public async Task<Brewer> UpdateAsync(Brewer brewer)
        {
            _context.Brewers.Update(brewer);
            await _context.SaveChangesAsync();
            return brewer;
        }

        public async Task DeleteAsync(Brewer brewer)
        {
            _context.Brewers.Remove(brewer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid brewerId, Guid userId)
        {
            return await _context.Brewers
                .AnyAsync(b => b.Id == brewerId && b.UserId == userId);
        }
    }
}