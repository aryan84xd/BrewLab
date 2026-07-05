using BrewLab.Models.Entities;

namespace BrewLab.Repositories.Interfaces
{
    public interface IBrewerRepository
    {
        Task<IEnumerable<Brewer>> GetAllAsync(Guid userId);

        Task<Brewer?> GetByIdAsync(Guid brewerId);

        Task<Brewer> CreateAsync(Brewer brewer);

        Task<Brewer> UpdateAsync(Brewer brewer);

        Task DeleteAsync(Brewer brewer);

        Task<bool> ExistsAsync(Guid brewerId, Guid userId);
    }
}