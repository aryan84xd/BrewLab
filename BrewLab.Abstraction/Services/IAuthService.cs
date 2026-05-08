using BrewLab.DomainModel.Contracts.ApiModels;
using BrewLab.DomainModel.Contracts.UserDTO;
using BrewLab.DomainModel.DBModels;

namespace BrewLab.Abstraction.Services
{
    public interface IAuthService
    {
        Task<DTOUserLoginResponse> RegisterAsync(DTOUserRegisterRequest request);
        Task<DTOUserLoginResponse?> LoginAsync(DTOUserLoginRequest request);
        Task<UserDBO?> GetUserByIdAsync(Guid id);
    }
}
