using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;

namespace BrewLab.Services.Interfaces
{
    public interface IExperimentService
    {
        Task<IEnumerable<ExperimentResponseModel>> GetAllAsync(Guid coffeeId);

        Task<ExperimentResponseModel> GetByIdAsync(Guid experimentId);

        Task<ExperimentResponseModel> CreateAsync(CreateExperimentRequestModel request);

        Task<ExperimentResponseModel> UpdateAsync(
            Guid experimentId,
            UpdateExperimentRequestModel request);
    }
}