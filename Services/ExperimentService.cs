using BrewLab.Models.DBO;
using BrewLab.Models.Requests;
using BrewLab.Models.Responses;
using BrewLab.Repositories;

namespace BrewLab.Services
{
    public interface IExperimentService
    {
        Task<IEnumerable<ExperimentResponse>> GetByCoffeeIdAsync(Guid coffeeId, Guid userId);
        Task<ExperimentResponse> CreateAsync(CreateExperimentRequest request, Guid userId);
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

        public async Task<IEnumerable<ExperimentResponse>> GetByCoffeeIdAsync(Guid coffeeId, Guid userId)
        {
            var coffeeExists = await _coffeeRepository.ExistsAsync(coffeeId, userId);
            if (!coffeeExists)
                throw new InvalidOperationException("Coffee not found for this user.");

            var experiments = await _experimentRepository.GetByCoffeeIdAsync(coffeeId, userId);
            return experiments.Select(e => new ExperimentResponse
            {
                Id = e.Id,
                CoffeeId = e.CoffeeId,
                Date = e.Date,
                BrewMethod = e.BrewMethod,
                CoffeeWeight = e.CoffeeWeight,
                WaterWeight = e.WaterWeight,
                BrewTime = e.BrewTime,
                Remark = e.Remark,
                Aroma = e.Aroma,
                Acidity = e.Acidity,
                Body = e.Body,
                Overall = e.Overall
            });
        }

        public async Task<ExperimentResponse> CreateAsync(CreateExperimentRequest request, Guid userId)
        {
            var coffeeExists = await _coffeeRepository.ExistsAsync(request.CoffeeId, userId);
            if (!coffeeExists)
                throw new InvalidOperationException("Coffee not found for the user.");

            var experiment = new ExperimentDBO
            {
                CoffeeId = request.CoffeeId,
                UserId = userId,
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

            await _experimentRepository.CreateAsync(experiment);

            return new ExperimentResponse
            {
                Id = experiment.Id,
                CoffeeId = experiment.CoffeeId,
                Date = experiment.Date,
                BrewMethod = experiment.BrewMethod,
                CoffeeWeight = experiment.CoffeeWeight,
                WaterWeight = experiment.WaterWeight,
                BrewTime = experiment.BrewTime,
                Remark = experiment.Remark,
                Aroma = experiment.Aroma,
                Acidity = experiment.Acidity,
                Body = experiment.Body,
                Overall = experiment.Overall
            };
        }
    }
}
