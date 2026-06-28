using BrewLab.Models.Entities;

namespace BrewLab.Repositories.Interfaces
{
    public interface IExperimentRepository
    {
        Task<IEnumerable<Experiment>> GetAllByCoffeeIdAsync(Guid coffeeId, Guid userId);

        Task<Experiment?> GetByIdAsync(Guid experimentId);

        Task<Experiment> CreateAsync(Experiment experiment);

        Task<Experiment> UpdateAsync(Experiment experiment);

        Task<bool> ExistsAsync(Guid experimentId, Guid userId);

        Task SaveChangesAsync();
        Task RemoveParametersAsync(Guid experimentId);
    }
}