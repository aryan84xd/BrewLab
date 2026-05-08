using BrewLab.Abstraction.Services;
using BrewLab.DomainModel.Contracts.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrewLab.ApiServices.Controllers
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
        public async Task<ActionResult<IEnumerable<CoffeeResponse>>> GetCoffees()
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var coffees = await _coffeeService.GetAllByUserIdAsync(userId.Value);
            return Ok(coffees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeResponse>> GetCoffee(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var coffee = await _coffeeService.GetByIdAsync(id, userId.Value);
            if (coffee is null)
                return NotFound();

            return Ok(coffee);
        }

        [HttpPost]
        public async Task<ActionResult<CoffeeResponse>> PostCoffee(CreateCoffeeRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var coffee = await _coffeeService.CreateAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetCoffee), new { id = coffee.Id }, coffee);
        }
    }
}
