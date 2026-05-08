using BrewLab.Abstraction.Repositories;
using BrewLab.Abstraction.Services;
using BrewLab.DomainModel.Contracts.ApiModels;
using BrewLab.DomainModel.DBModels;

namespace BrewLab.BusinessLogic.Services
{
    public class CoffeeService : ICoffeeService
    {
        private readonly ICoffeeRepository _coffeeRepository;

        public CoffeeService(ICoffeeRepository coffeeRepository)
        {
            _coffeeRepository = coffeeRepository;
        }

        public async Task<CoffeeResponse?> GetByIdAsync(Guid id, Guid userId)
        {
            var coffee = await _coffeeRepository.GetByIdAsync(id, userId);
            if (coffee is null)
                return null;

            return new CoffeeResponse
            {
                Id = coffee.Id,
                Name = coffee.Name,
                Brand = coffee.Brand,
                Roast = coffee.Roast,
                Origin = coffee.Origin,
                TastingNotes = coffee.TastingNotes
            };
        }

        public async Task<IEnumerable<CoffeeResponse>> GetAllByUserIdAsync(Guid userId)
        {
            var coffees = await _coffeeRepository.GetAllByUserIdAsync(userId);
            return coffees.Select(c => new CoffeeResponse
            {
                Id = c.Id,
                Name = c.Name,
                Brand = c.Brand,
                Roast = c.Roast,
                Origin = c.Origin,
                TastingNotes = c.TastingNotes
            });
        }

        public async Task<CoffeeResponse> CreateAsync(CreateCoffeeRequest request, Guid userId)
        {
            var coffee = new CoffeeDBO
            {
                Name = request.Name,
                Brand = request.Brand,
                Roast = request.Roast,
                Origin = request.Origin,
                TastingNotes = request.TastingNotes,
                UserId = userId
            };

            await _coffeeRepository.CreateAsync(coffee);

            return new CoffeeResponse
            {
                Id = coffee.Id,
                Name = coffee.Name,
                Brand = coffee.Brand,
                Roast = coffee.Roast,
                Origin = coffee.Origin,
                TastingNotes = coffee.TastingNotes
            };
        }
    }
}
