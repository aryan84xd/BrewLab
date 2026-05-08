using BrewLab.DomainModel.DBModels;

namespace BrewLab.Abstraction.Repositories
{
    public interface IUserRepository
    {
        Task<UserDBO?> GetByIdAsync(Guid id);
        Task<UserDBO?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<UserDBO> CreateAsync(UserDBO user);
    }
}
