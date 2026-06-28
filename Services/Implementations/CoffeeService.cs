using BrewLab.Authentication;
using BrewLab.Models.Entities;
using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;
using BrewLab.Repositories.Interfaces;
using BrewLab.Services.Interfaces;

namespace BrewLab.Services.Implementations
{
    public class CoffeeService : ICoffeeService
    {
        private readonly ICoffeeRepository _coffeeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ResponseFactory _responseFactory;

        public CoffeeService(
            ICoffeeRepository coffeeRepository,
            ICurrentUserService currentUserService,
            ResponseFactory responseFactory)
        {
            _coffeeRepository = coffeeRepository ?? throw new ArgumentNullException(nameof(coffeeRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
        }
        

        public async Task<IEnumerable<CoffeeResponseModel>> GetAllAsync()
        {
            var userId = _currentUserService.UserId;
            var coffees = await _coffeeRepository.GetAllByUserIdAsync(userId);

            return coffees.Select(MapToResponseModel);
        }

        public async Task<CoffeeResponseModel> GetByIdAsync(Guid coffeeId)
        {
            if (coffeeId == Guid.Empty)
            {
                return _responseFactory.Failure<CoffeeResponseModel>("InvalidCoffeeId", "Coffee ID is required");
            }
            var coffee = await _coffeeRepository.GetByIdAsync(coffeeId);
            if (coffee == null)
            {
                return _responseFactory.Failure<CoffeeResponseModel>("CoffeeNotFound", "Coffee not found");
            }

            // Verify ownership
            var userId = _currentUserService.UserId;
            if (coffee.UserId != userId)
            {
                return _responseFactory.Failure<CoffeeResponseModel>("Unauthorized", "You do not have access to this coffee");
            }
            

            return MapToResponseModel(coffee);
        }

        public async Task<CoffeeResponseModel> CreateAsync(CreateCoffeeRequestModel request)
        {
            var response = new CoffeeResponseModel();

            if (request == null)
            {
                return _responseFactory.Failure<CoffeeResponseModel>("InvalidRequest", "Coffee request cannot be null");
            }

            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return _responseFactory.Failure<CoffeeResponseModel>("InvalidName", "Coffee name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Brand))
            {
                return _responseFactory.Failure<CoffeeResponseModel>("InvalidBrand", "Coffee brand is required");
            }

            try
            {
                // Get authenticated user ID
                var userId = _currentUserService.UserId;

                // Create coffee entity
                var coffee = new Coffee
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Brand = request.Brand,
                    Roast = request.Roast,
                    Origin = request.Origin,
                    TastingNotes = request.TastingNotes,
                    RoastDate = request.RoastDate,
                    Process = request.Process,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                await _coffeeRepository.CreateAsync(coffee);

                return MapToResponseModel(coffee);
            }
            catch (Exception ex)
            {
              return _responseFactory.Failure<CoffeeResponseModel>("CreationFailed", $"An error occurred while creating the coffee: {ex.Message}");
            }
        }

        public async Task<CoffeeResponseModel> UpdateAsync(Guid coffeeId, UpdateCoffeeRequestModel request)
        {
            var response = new CoffeeResponseModel();

            if (coffeeId == Guid.Empty)
            {
               return _responseFactory.Failure<CoffeeResponseModel>("InvalidCoffeeId", "Coffee ID is required");
            }

            if (request == null)
            {
                return _responseFactory.Failure<CoffeeResponseModel>("InvalidRequest", "Coffee request cannot be null");
            }


            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return _responseFactory.Failure<CoffeeResponseModel>("InvalidName", "Coffee name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Brand))
            {
                return _responseFactory.Failure<CoffeeResponseModel>("InvalidBrand", "Coffee brand is required");
            }

            try
            {
                var userId = _currentUserService.UserId;

                // Verify ownership
                var exists = await _coffeeRepository.ExistsAsync(coffeeId, userId);
                if (!exists)
                {
                    return _responseFactory.Failure<CoffeeResponseModel>("Unauthorized", "You do not have access to this coffee");
                }

                // Get current coffee to preserve CreatedAt
                var coffee = await _coffeeRepository.GetByIdAsync(coffeeId);
                if (coffee == null)
                {
                    return _responseFactory.Failure<CoffeeResponseModel>("CoffeeNotFound", "Coffee not found");
                }
                

                // Update properties
                coffee.Name = request.Name;
                coffee.Brand = request.Brand;
                coffee.Roast = request.Roast;
                coffee.Origin = request.Origin;
                coffee.TastingNotes = request.TastingNotes;
                coffee.RoastDate = request.RoastDate;
                coffee.Process = request.Process;

                // Update in database
                await _coffeeRepository.UpdateAsync(coffee);

                return MapToResponseModel(coffee);
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<CoffeeResponseModel>("UpdateFailed", $"An error occurred while updating the coffee: {ex.Message}");
            }
        }

        private static CoffeeResponseModel MapToResponseModel(Coffee coffee)
        {
            return new CoffeeResponseModel
            {
                Success = true,
                Id = coffee.Id,
                CreatedAt = coffee.CreatedAt,
                Name = coffee.Name,
                Brand = coffee.Brand,
                Roast = coffee.Roast,
                Origin = coffee.Origin,
                TastingNotes = coffee.TastingNotes,
                RoastDate = coffee.RoastDate,
                Process = coffee.Process
            };
        }
    }
}
