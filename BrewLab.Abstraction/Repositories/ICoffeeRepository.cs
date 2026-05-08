using BrewLab.DomainModel.DBModels;

namespace BrewLab.Abstraction.Repositories
{
    public interface ICoffeeRepository
    {
        Task<CoffeeDBO?> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<CoffeeDBO>> GetAllByUserIdAsync(Guid userId);
        Task<CoffeeDBO> CreateAsync(CoffeeDBO coffee);
        Task<bool> ExistsAsync(Guid id, Guid userId);
    }
}
