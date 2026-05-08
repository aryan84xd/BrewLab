using BrewLab.DomainModel.Contracts.ApiModels;

namespace BrewLab.Abstraction.Services
{
    public interface IExperimentService
    {
        Task<IEnumerable<ExperimentResponse>> GetByCoffeeIdAsync(Guid coffeeId, Guid userId);
        Task<ExperimentResponse> CreateAsync(CreateExperimentRequest request, Guid userId);
    }
}
