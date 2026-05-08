using BrewLab.DomainModel.Contracts.ApiModels;

namespace BrewLab.Abstraction.Services
{
    public interface ICoffeeService
    {
        Task<CoffeeResponse?> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<CoffeeResponse>> GetAllByUserIdAsync(Guid userId);
        Task<CoffeeResponse> CreateAsync(CreateCoffeeRequest request, Guid userId);
    }
}
