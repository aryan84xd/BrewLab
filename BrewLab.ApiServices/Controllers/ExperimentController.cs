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
    public class ExperimentController : ControllerBase
    {
        private readonly IExperimentService _experimentService;

        public ExperimentController(IExperimentService experimentService)
        {
            _experimentService = experimentService;
        }

        private Guid? GetCurrentUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !Guid.TryParse(userId, out var id))
                return null;

            return id;
        }

        [HttpGet("{coffeeId}")]
        public async Task<ActionResult<IEnumerable<ExperimentResponse>>> GetCoffeeExperiments(Guid coffeeId)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            try
            {
                var experiments = await _experimentService.GetByCoffeeIdAsync(coffeeId, userId.Value);
                return Ok(experiments);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ExperimentResponse>> PostExperiment(CreateExperimentRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            try
            {
                var experiment = await _experimentService.CreateAsync(request, userId.Value);
                return Ok(experiment);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
