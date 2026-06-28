using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;
using BrewLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoffeesController : ControllerBase
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeesController(ICoffeeService coffeeService)
        {
            _coffeeService = coffeeService ?? throw new ArgumentNullException(nameof(coffeeService));
        }

        /// <summary>
        /// Get all coffees belonging to the authenticated user
        /// </summary>
        /// <returns>List of coffees</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CoffeeResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var coffees = await _coffeeService.GetAllAsync();
            return Ok(coffees);
        }

        /// <summary>
        /// Get a specific coffee belonging to the authenticated user
        /// </summary>
        /// <param name="id">Coffee ID</param>
        /// <returns>Coffee details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _coffeeService.GetByIdAsync(id);

            if (!result.Success)
            {
                if (result.Error == "Unauthorized")
                    return Unauthorized(result);

                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new coffee for the authenticated user
        /// </summary>
        /// <param name="request">Coffee details</param>
        /// <returns>Created coffee</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCoffeeRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _coffeeService.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update a coffee belonging to the authenticated user
        /// </summary>
        /// <param name="id">Coffee ID</param>
        /// <param name="request">Updated coffee details</param>
        /// <returns>Updated coffee</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CoffeeResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCoffeeRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _coffeeService.UpdateAsync(id, request);

            if (!result.Success)
            {
                if (result.Error == "Unauthorized")
                    return Unauthorized(result);

                if (result.Error == "CoffeeNotFound")
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
