using BrewLab.Models.DBO;
using BrewLab.Models.DTOs.ExperimentDTO;
using BrewLab.Models.Requests;
using BrewLab.Repositories;

namespace BrewLab.Services
{
    public interface IExperimentService
    {
        Task<(bool Success, string? ErrorMessage, IEnumerable<DTOExperiment>? Data)> GetByCoffeeIdAsync(Guid coffeeId, Guid userId);
        Task<(bool Success, string? ErrorMessage, DTOExperiment? Data)> CreateAsync(CreateExperimentRequest request, Guid userId);
    }

    public class ExperimentService : IExperimentService
    {
        private readonly IExperimentRepository _experimentRepository;
        private readonly ICoffeeRepository _coffeeRepository;

        public ExperimentService(IExperimentRepository experimentRepository, ICoffeeRepository coffeeRepository)
        {
            _experimentRepository = experimentRepository;
            _coffeeRepository = coffeeRepository;
        }

        public async Task<(bool Success, string? ErrorMessage, IEnumerable<DTOExperiment>? Data)> GetByCoffeeIdAsync(Guid coffeeId, Guid userId)
        {
            var coffeeExists = await _coffeeRepository.ExistsAsync(coffeeId, userId);
            if (!coffeeExists)
                return (false, "Coffee not found for this user.", null);

            var experimentDboList = await _experimentRepository.GetByCoffeeIdAsync(coffeeId, userId);
            var dtos = experimentDboList.Select(MapDboToDto);
            return (true, null, dtos);
        }

        public async Task<(bool Success, string? ErrorMessage, DTOExperiment? Data)> CreateAsync(CreateExperimentRequest request, Guid userId)
        {
            var coffeeExists = await _coffeeRepository.ExistsAsync(request.CoffeeId, userId);
            if (!coffeeExists)
                return (false, "Coffee not found for the user.", null);

            var experimentDbo = MapRequestToDbo(request, userId);
            var createdDbo = await _experimentRepository.CreateAsync(experimentDbo);
            var dto = MapDboToDto(createdDbo);
            return (true, null, dto);
        }

        private static DTOExperiment MapDboToDto(ExperimentDBO dbo)
        {
            return new DTOExperiment
            {
                Id = dbo.Id,
                CoffeeId = dbo.CoffeeId,
                Date = dbo.Date,
                BrewMethod = dbo.BrewMethod,
                CoffeeWeight = dbo.CoffeeWeight,
                WaterWeight = dbo.WaterWeight,
                BrewTime = dbo.BrewTime,
                Remark = dbo.Remark,
                Aroma = dbo.Aroma,
                Acidity = dbo.Acidity,
                Body = dbo.Body,
                Overall = dbo.Overall,
                UserId = dbo.UserId
            };
        }

        private static ExperimentDBO MapRequestToDbo(CreateExperimentRequest request, Guid userId)
        {
            return new ExperimentDBO
            {
                CoffeeId = request.CoffeeId,
                UserId = userId,
                Date = DateTime.UtcNow,
                BrewMethod = request.BrewMethod,
                CoffeeWeight = request.CoffeeWeight,
                WaterWeight = request.WaterWeight,
                BrewTime = request.BrewTime,
                Remark = request.Remark,
                Aroma = request.Aroma,
                Acidity = request.Acidity,
                Body = request.Body,
                Overall = request.Overall
            };
        }
    }
}
