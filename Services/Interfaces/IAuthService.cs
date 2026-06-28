using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;

namespace BrewLab.Services.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse> RegisterAsync(RegisterRequestModel request);
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request);
    }
}
