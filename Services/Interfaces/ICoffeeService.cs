using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;

namespace BrewLab.Services.Interfaces
{
    public interface ICoffeeService
    {
        Task<IEnumerable<CoffeeResponseModel>> GetAllAsync();
        Task<CoffeeResponseModel> GetByIdAsync(Guid coffeeId);
        Task<CoffeeResponseModel> CreateAsync(CreateCoffeeRequestModel request);
        Task<CoffeeResponseModel> UpdateAsync(Guid coffeeId, UpdateCoffeeRequestModel request);
    }
}
