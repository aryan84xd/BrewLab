using BrewLab.DomainModel.DBModels;

namespace BrewLab.Abstraction.Repositories
{
    public interface IExperimentRepository
    {
        Task<ExperimentDBO?> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<ExperimentDBO>> GetByCoffeeIdAsync(Guid coffeeId, Guid userId);
        Task<ExperimentDBO> CreateAsync(ExperimentDBO experiment);
    }
}
