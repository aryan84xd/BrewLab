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
        public async Task<ActionResult<ApiResponse<IEnumerable<ExperimentResponse>>>> GetCoffeeExperiments(Guid coffeeId)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Ok(ApiResponse<IEnumerable<ExperimentResponse>>.FailureResponse("Unauthorized access."));

            var (success, errorMessage, dtos) = await _experimentService.GetByCoffeeIdAsync(coffeeId, userId.Value);

            if (!success)
                return Ok(ApiResponse<IEnumerable<ExperimentResponse>>.FailureResponse(errorMessage!));

            var responses = dtos!.Select(dto => new ExperimentResponse
            {
                Id = dto.Id,
                CoffeeId = dto.CoffeeId,
                Date = dto.Date,
                BrewMethod = dto.BrewMethod,
                CoffeeWeight = dto.CoffeeWeight,
                WaterWeight = dto.WaterWeight,
                BrewTime = dto.BrewTime,
                Remark = dto.Remark,
                Aroma = dto.Aroma,
                Acidity = dto.Acidity,
                Body = dto.Body,
                Overall = dto.Overall
            });

            return Ok(ApiResponse<IEnumerable<ExperimentResponse>>.SuccessResponse(responses));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExperimentResponse>>> PostExperiment(CreateExperimentRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Ok(ApiResponse<ExperimentResponse>.FailureResponse("Unauthorized access."));

            var (success, errorMessage, dto) = await _experimentService.CreateAsync(request, userId.Value);

            if (!success)
                return Ok(ApiResponse<ExperimentResponse>.FailureResponse(errorMessage!));

            var response = new ExperimentResponse
            {
                Id = dto!.Id,
                CoffeeId = dto.CoffeeId,
                Date = dto.Date,
                BrewMethod = dto.BrewMethod,
                CoffeeWeight = dto.CoffeeWeight,
                WaterWeight = dto.WaterWeight,
                BrewTime = dto.BrewTime,
                Remark = dto.Remark,
                Aroma = dto.Aroma,
                Acidity = dto.Acidity,
                Body = dto.Body,
                Overall = dto.Overall
            };

            return Ok(ApiResponse<ExperimentResponse>.SuccessResponse(response));
        }
    }
}
