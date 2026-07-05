using BrewLab.Models.Entities;

namespace BrewLab.Repositories.Interfaces
{
    public interface IGrinderRepository
    {
        Task<IEnumerable<Grinder>> GetAllAsync(Guid userId);

        Task<Grinder?> GetByIdAsync(Guid grinderId);

        Task<Grinder> CreateAsync(Grinder grinder);

        Task<Grinder> UpdateAsync(Grinder grinder);

        Task DeleteAsync(Grinder grinder);

        Task<bool> ExistsAsync(Guid grinderId, Guid userId);
    }
}