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
    public class ExperimentsController : ControllerBase
    {
        private readonly IExperimentService _experimentService;

        public ExperimentsController(IExperimentService experimentService)
        {
            _experimentService = experimentService ?? throw new ArgumentNullException(nameof(experimentService));
        }

        /// <summary>
        /// Get all experiments for a coffee belonging to the authenticated user
        /// </summary>
        [HttpGet("/api/coffees/{coffeeId:guid}/experiments")]
        [ProducesResponseType(typeof(IEnumerable<ExperimentResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(Guid coffeeId)
        {
            var experiments = await _experimentService.GetAllAsync(coffeeId);
            return Ok(experiments);
        }

        /// <summary>
        /// Get a specific experiment belonging to the authenticated user
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _experimentService.GetByIdAsync(id);

            if (!result.Success)
            {
                if (result.Error == "Unauthorized")
                    return Unauthorized(result);

                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new experiment
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateExperimentRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _experimentService.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing experiment
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExperimentResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateExperimentRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _experimentService.UpdateAsync(id, request);

            if (!result.Success)
            {
                if (result.Error == "Unauthorized")
                    return Unauthorized(result);

                if (result.Error == "ExperimentNotFound")
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}