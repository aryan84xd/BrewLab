using BrewLab.Models.User;

namespace BrewLab.Services
{
    public interface IUserService
    {
        Task<UserLoginResponse> RegisterAsync(UserRegisterModel request);
        Task<UserLoginResponse> LoginAsync(UserLoginRequest request);
    }
}
