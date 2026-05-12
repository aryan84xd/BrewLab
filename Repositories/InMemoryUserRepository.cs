using BrewLab.Data;
using BrewLab.Models.DBO;

namespace BrewLab.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly IInMemoryDatabase _db;

        public InMemoryUserRepository(IInMemoryDatabase db)
        {
            _db = db;
        }

        public Task<UserDBO?> GetByIdAsync(Guid id)
        {
            _db.Users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task<UserDBO?> GetByEmailAsync(string email)
        {
            var user = _db.Users.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            var exists = _db.Users.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }

        public Task<UserDBO> CreateAsync(UserDBO user)
        {
            user.Id = Guid.NewGuid();
            _db.Users.TryAdd(user.Id, user);
            return Task.FromResult(user);
        }
    }
}
