
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
    public class GrindersController : ControllerBase
    {
        private readonly IGrinderService _grinderService;

        public GrindersController(IGrinderService grinderService)
        {
            _grinderService = grinderService ?? throw new ArgumentNullException(nameof(grinderService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var grinders = await _grinderService.GetAllAsync();
            return Ok(grinders);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _grinderService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGrinderRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _grinderService.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateGrinderRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _grinderService.UpdateAsync(id, request);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _grinderService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return NoContent();
        }
    }
}
