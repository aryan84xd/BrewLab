using BrewLab.Models.DBO;
using BrewLab.Models.DTOs.CoffeeDTO;
using BrewLab.Models.Requests;
using BrewLab.Models.Responses;
using BrewLab.Repositories;

namespace BrewLab.Services
{
    public interface ICoffeeService
    {
        Task<DTOCoffee?> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<DTOCoffee>> GetAllByUserIdAsync(Guid userId);
        Task<DTOCoffee> CreateAsync(CreateCoffeeRequest request, Guid userId);
    }

    public class CoffeeService : ICoffeeService
    {
        private readonly ICoffeeRepository _coffeeRepository;

        public CoffeeService(ICoffeeRepository coffeeRepository)
        {
            _coffeeRepository = coffeeRepository;
        }

        public async Task<DTOCoffee?> GetByIdAsync(Guid id, Guid userId)
        {
            var coffeeDbo = await _coffeeRepository.GetByIdAsync(id, userId);
            if (coffeeDbo is null)
                return null;

            return MapDboToDto(coffeeDbo);
        }

        public async Task<IEnumerable<DTOCoffee>> GetAllByUserIdAsync(Guid userId)
        {
            var coffeeDboList = await _coffeeRepository.GetAllByUserIdAsync(userId);
            return coffeeDboList.Select(MapDboToDto);
        }

        public async Task<DTOCoffee> CreateAsync(CreateCoffeeRequest request, Guid userId)
        {
            var coffeeDbo = MapRequestToDbo(request, userId);
            var createdDbo = await _coffeeRepository.CreateAsync(coffeeDbo);
            return MapDboToDto(createdDbo);
        }

        private static DTOCoffee MapDboToDto(CoffeeDBO dbo)
        {
            return new DTOCoffee
            {
                Id = dbo.Id,
                Name = dbo.Name,
                Brand = dbo.Brand,
                Roast = dbo.Roast,
                Origin = dbo.Origin,
                TastingNotes = dbo.TastingNotes,
                UserId = dbo.UserId
            };
        }

        private static CoffeeDBO MapRequestToDbo(CreateCoffeeRequest request, Guid userId)
        {
            return new CoffeeDBO
            {
                Name = request.Name,
                Brand = request.Brand,
                Roast = request.Roast,
                Origin = request.Origin,
                TastingNotes = request.TastingNotes,
                UserId = userId
            };
        }
    }
}
