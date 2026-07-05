
using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;

public interface IGrinderService


{
    Task<IEnumerable<GrinderResponseModel>> GetAllAsync();

    Task<GrinderResponseModel> GetByIdAsync(Guid grinderId);

    Task<GrinderResponseModel> CreateAsync(CreateGrinderRequestModel request);

    Task<GrinderResponseModel> UpdateAsync(
        Guid grinderId,
        UpdateGrinderRequestModel request);

    Task<BaseResponse> DeleteAsync(Guid grinderId);
}