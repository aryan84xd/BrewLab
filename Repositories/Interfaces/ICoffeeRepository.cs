using BrewLab.Models.Entities;

namespace BrewLab.Repositories.Interfaces
{
    public interface ICoffeeRepository
    {
        Task<IEnumerable<Coffee>> GetAllByUserIdAsync(Guid userId);
        Task<Coffee?> GetByIdAsync(Guid coffeeId);
        Task<Coffee> CreateAsync(Coffee coffee);
        Task<Coffee> UpdateAsync(Coffee coffee);
        Task<bool> ExistsAsync(Guid coffeeId, Guid userId);
        Task SaveChangesAsync();
    }
}
