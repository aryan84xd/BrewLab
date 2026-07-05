using BrewLab.Data;
using BrewLab.Models.Entities;
using BrewLab.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewLab.Repositories.Implementations
{
    public class ExperimentRepository : IExperimentRepository
    {
        private readonly AppDbContext _context;

        public ExperimentRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Experiment>> GetAllByCoffeeIdAsync(Guid coffeeId, Guid userId)
        {
            if (coffeeId == Guid.Empty || userId == Guid.Empty)
                return Enumerable.Empty<Experiment>();

            return await _context.Experiments
                .AsNoTracking()
                .Include(e => e.BrewMethod)
                .Where(e => e.CoffeeId == coffeeId && e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Experiment?> GetByIdAsync(Guid experimentId)
        {
            if (experimentId == Guid.Empty)
                return null;

            return await _context.Experiments
                .Include(e => e.Brewer)
                .Include(e => e.Grinder)
                .Include(e => e.BrewMethod)
                .Include(e => e.Parameters)
                    .ThenInclude(p => p.BrewParameter)
                .FirstOrDefaultAsync(e => e.Id == experimentId);
        }

        public async Task<Experiment> CreateAsync(Experiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            _context.Experiments.Add(experiment);

            await _context.SaveChangesAsync();

            return experiment;
        }

        public async Task<Experiment> UpdateAsync(Experiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            _context.Experiments.Update(experiment);

            await _context.SaveChangesAsync();

            return experiment;
        }

        public async Task<bool> ExistsAsync(Guid experimentId, Guid userId)
        {
            if (experimentId == Guid.Empty || userId == Guid.Empty)
                return false;

            return await _context.Experiments
                .AnyAsync(e => e.Id == experimentId && e.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task RemoveParametersAsync(Guid experimentId)
        {
            var parameters = await _context.ExperimentParameters
                .Where(p => p.ExperimentId == experimentId)
                .ToListAsync();

            _context.ExperimentParameters.RemoveRange(parameters);
        }
    }
}