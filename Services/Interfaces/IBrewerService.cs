using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;

public interface IBrewerService
{
    Task<IEnumerable<BrewerResponseModel>> GetAllAsync();

    Task<BrewerResponseModel> GetByIdAsync(Guid brewerId);

    Task<BrewerResponseModel> CreateAsync(CreateBrewerRequestModel request);

    Task<BrewerResponseModel> UpdateAsync(
        Guid brewerId,
        UpdateBrewerRequestModel request);

    Task<BaseResponse> DeleteAsync(Guid brewerId);
}