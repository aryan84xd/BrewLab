
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
    public class BrewersController : ControllerBase
    {
        private readonly IBrewerService _brewerService;

        public BrewersController(IBrewerService brewerService)
        {
            _brewerService = brewerService ?? throw new ArgumentNullException(nameof(brewerService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrewerResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var brewers = await _brewerService.GetAllAsync();
            return Ok(brewers);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BrewerResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BrewerResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _brewerService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BrewerResponseModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateBrewerRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _brewerService.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BrewerResponseModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, UpdateBrewerRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _brewerService.UpdateAsync(id, request);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _brewerService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return NoContent();
        }
    }
}