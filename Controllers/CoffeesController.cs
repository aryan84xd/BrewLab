using BrewLab.Models.Common;
using BrewLab.Models.Requests;
using BrewLab.Models.Responses;
using BrewLab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrewLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoffeesController : ControllerBase
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeesController(ICoffeeService coffeeService)
        {
            _coffeeService = coffeeService;
        }

        private Guid? GetCurrentUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !Guid.TryParse(userId, out var id))
                return null;

            return id;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CoffeeResponse>>>> GetCoffees()
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Ok(ApiResponse<IEnumerable<CoffeeResponse>>.FailureResponse("Unauthorized access."));

            var dtos = await _coffeeService.GetAllByUserIdAsync(userId.Value);
            var responses = dtos.Select(dto => new CoffeeResponse
            {
                Id = dto.Id,
                Name = dto.Name,
                Brand = dto.Brand,
                Roast = dto.Roast,
                Origin = dto.Origin,
                TastingNotes = dto.TastingNotes
            });

            return Ok(ApiResponse<IEnumerable<CoffeeResponse>>.SuccessResponse(responses));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CoffeeResponse>>> GetCoffee(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Ok(ApiResponse<CoffeeResponse>.FailureResponse("Unauthorized access."));

            var dto = await _coffeeService.GetByIdAsync(id, userId.Value);
            if (dto is null)
                return Ok(ApiResponse<CoffeeResponse>.FailureResponse("Coffee not found."));

            var response = new CoffeeResponse
            {
                Id = dto.Id,
                Name = dto.Name,
                Brand = dto.Brand,
                Roast = dto.Roast,
                Origin = dto.Origin,
                TastingNotes = dto.TastingNotes
            };

            return Ok(ApiResponse<CoffeeResponse>.SuccessResponse(response));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CoffeeResponse>>> PostCoffee(CreateCoffeeRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Ok(ApiResponse<CoffeeResponse>.FailureResponse("Unauthorized access."));

            var dto = await _coffeeService.CreateAsync(request, userId.Value);
            var response = new CoffeeResponse
            {
                Id = dto.Id,
                Name = dto.Name,
                Brand = dto.Brand,
                Roast = dto.Roast,
                Origin = dto.Origin,
                TastingNotes = dto.TastingNotes
            };

            return Ok(ApiResponse<CoffeeResponse>.SuccessResponse(response));
        }
    }
}
