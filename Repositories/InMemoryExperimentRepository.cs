using BrewLab.Data;
using BrewLab.Models.DBO;

namespace BrewLab.Repositories
{
    public class InMemoryExperimentRepository : IExperimentRepository
    {
        private readonly IInMemoryDatabase _db;

        public InMemoryExperimentRepository(IInMemoryDatabase db)
        {
            _db = db;
        }

        public Task<ExperimentDBO?> GetByIdAsync(Guid id, Guid userId)
        {
            _db.Experiments.TryGetValue(id, out var experiment);
            if (experiment != null && experiment.UserId == userId)
            {
                return Task.FromResult<ExperimentDBO?>(experiment);
            }
            return Task.FromResult<ExperimentDBO?>(null);
        }

        public Task<IEnumerable<ExperimentDBO>> GetByCoffeeIdAsync(Guid coffeeId, Guid userId)
        {
            var experiments = _db.Experiments.Values
                .Where(e => e.CoffeeId == coffeeId && e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToList();
            return Task.FromResult<IEnumerable<ExperimentDBO>>(experiments);
        }

        public Task<ExperimentDBO> CreateAsync(ExperimentDBO experiment)
        {
            experiment.Id = Guid.NewGuid();
            experiment.Date = DateTime.UtcNow;
            _db.Experiments.TryAdd(experiment.Id, experiment);
            return Task.FromResult(experiment);
        }
    }
}
