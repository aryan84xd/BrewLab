using BrewLab.Authentication;
using BrewLab.Models.Entities;
using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;
using BrewLab.Repositories.Interfaces;
using BrewLab.Services.Interfaces;

namespace BrewLab.Services.Implementations
{
    public class ExperimentService : IExperimentService
    {
        private readonly IExperimentRepository _experimentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ResponseFactory _responseFactory;

        public ExperimentService(
            IExperimentRepository experimentRepository,
            ICurrentUserService currentUserService,
            ResponseFactory responseFactory)
        {
            _experimentRepository = experimentRepository ?? throw new ArgumentNullException(nameof(experimentRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
        }

        public async Task<IEnumerable<ExperimentResponseModel>> GetAllAsync(Guid coffeeId)
        {
            if (coffeeId == Guid.Empty)
                return Enumerable.Empty<ExperimentResponseModel>();

            var userId = _currentUserService.UserId;

            var experiments = await _experimentRepository.GetAllByCoffeeIdAsync(coffeeId, userId);

            return experiments.Select(MapToResponseModel);
        }
        public async Task<ExperimentResponseModel> GetByIdAsync(Guid experimentId)
        {
            if (experimentId == Guid.Empty)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "InvalidExperimentId",
                    "Experiment ID is required");
            }

            var experiment = await _experimentRepository.GetByIdAsync(experimentId);

            if (experiment == null)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "ExperimentNotFound",
                    "Experiment not found");
            }

            var userId = _currentUserService.UserId;

            if (experiment.UserId != userId)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "Unauthorized",
                    "You do not have access to this experiment");
            }

            return MapToResponseModel(experiment);
        }
        public async Task<ExperimentResponseModel> CreateAsync(CreateExperimentRequestModel request)
        {
            if (request == null)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "InvalidRequest",
                    "Experiment request cannot be null");
            }

            if (request.CoffeeId == Guid.Empty)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "InvalidCoffeeId",
                    "Coffee ID is required");
            }

            if (request.BrewMethodId == Guid.Empty)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "InvalidBrewMethodId",
                    "Brew Method ID is required");
            }

            try
            {
                var userId = _currentUserService.UserId;

                var experiment = new Experiment
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,

                    CoffeeWeight = request.CoffeeWeight,
                    WaterWeight = request.WaterWeight,
                    WaterTemperature = request.WaterTemperature,
                    GrindSetting = request.GrindSetting,
                    BrewTime = request.BrewTime,
                    Remark = request.Remark,

                    Aroma = request.Aroma,
                    Acidity = request.Acidity,
                    Body = request.Body,
                    Sweetness = request.Sweetness,
                    Bitterness = request.Bitterness,
                    Aftertaste = request.Aftertaste,

                    Extraction = request.Extraction,
                    Overall = request.Overall,

                    CoffeeId = request.CoffeeId,
                    BrewMethodId = request.BrewMethodId,
                    UserId = userId
                };

                if (request.Parameters != null && request.Parameters.Any())
                {
                    foreach (var parameter in request.Parameters)
                    {
                        experiment.Parameters.Add(new ExperimentParameter
                        {
                            Id = Guid.NewGuid(),
                            ExperimentId = experiment.Id,
                            BrewParameterId = parameter.BrewParameterId,
                            Value = parameter.Value
                        });
                    }
                }

                await _experimentRepository.CreateAsync(experiment);

                return MapToResponseModel(experiment);
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "CreationFailed",
                    $"An error occurred while creating the experiment: {ex.Message}");
            }
        }

        public async Task<ExperimentResponseModel> UpdateAsync(
            Guid experimentId,
            UpdateExperimentRequestModel request)
        {
            if (experimentId == Guid.Empty)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "InvalidExperimentId",
                    "Experiment ID is required");
            }

            if (request == null)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "InvalidRequest",
                    "Experiment request cannot be null");
            }

            try
            {
                var userId = _currentUserService.UserId;

                var exists = await _experimentRepository.ExistsAsync(experimentId, userId);

                if (!exists)
                {
                    return _responseFactory.Failure<ExperimentResponseModel>(
                        "Unauthorized",
                        "You do not have access to this experiment");
                }

                var experiment = await _experimentRepository.GetByIdAsync(experimentId);

                if (experiment == null)
                {
                    return _responseFactory.Failure<ExperimentResponseModel>(
                        "ExperimentNotFound",
                        "Experiment not found");
                }

                experiment.CoffeeWeight = request.CoffeeWeight;
                experiment.WaterWeight = request.WaterWeight;
                experiment.WaterTemperature = request.WaterTemperature;
                experiment.GrindSetting = request.GrindSetting;
                experiment.BrewTime = request.BrewTime;
                experiment.Remark = request.Remark;

                experiment.Aroma = request.Aroma;
                experiment.Acidity = request.Acidity;
                experiment.Body = request.Body;
                experiment.Sweetness = request.Sweetness;
                experiment.Bitterness = request.Bitterness;
                experiment.Aftertaste = request.Aftertaste;

                experiment.Extraction = request.Extraction;
                experiment.Overall = request.Overall;

                experiment.CoffeeId = request.CoffeeId;
                experiment.BrewMethodId = request.BrewMethodId;
                await _experimentRepository.RemoveParametersAsync(experiment.Id);
                experiment.Parameters.Clear();

                if (request.Parameters != null && request.Parameters.Any())
                {
                    foreach (var parameter in request.Parameters)
                    {
                        experiment.Parameters.Add(new ExperimentParameter
                        {
                            Id = Guid.NewGuid(),
                            ExperimentId = experiment.Id,
                            BrewParameterId = parameter.BrewParameterId,
                            Value = parameter.Value
                        });
                    }
                }

                await _experimentRepository.UpdateAsync(experiment);

                return MapToResponseModel(experiment);
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<ExperimentResponseModel>(
                    "UpdateFailed",
                    $"An error occurred while updating the experiment: {ex.Message}");
            }
        }
        private static ExperimentResponseModel MapToResponseModel(Experiment experiment)
        {
            return new ExperimentResponseModel
            {
                Success = true,

                Id = experiment.Id,
                CreatedAt = experiment.CreatedAt,

                CoffeeWeight = experiment.CoffeeWeight,
                WaterWeight = experiment.WaterWeight,
                WaterTemperature = experiment.WaterTemperature,
                GrindSetting = experiment.GrindSetting,
                BrewTime = experiment.BrewTime,
                Remark = experiment.Remark,

                Aroma = experiment.Aroma,
                Acidity = experiment.Acidity,
                Body = experiment.Body,
                Sweetness = experiment.Sweetness,
                Bitterness = experiment.Bitterness,
                Aftertaste = experiment.Aftertaste,

                Extraction = experiment.Extraction,
                Overall = experiment.Overall,

                CoffeeId = experiment.CoffeeId,
                BrewMethodId = experiment.BrewMethodId,

                Parameters = experiment.Parameters
                    .Select(parameter => new ExperimentParameterResponseModel
                    {
                        BrewParameterId = parameter.BrewParameterId,
                        Value = parameter.Value
                    })
                    .ToList()
            };
        }

    }
}
    