using BrewLab.Data;
using BrewLab.Models.DBO;
using Npgsql;

namespace BrewLab.Repositories
{
    public interface IUserRepository
    {
        Task<UserDBO?> GetByIdAsync(Guid id);
        Task<UserDBO?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<UserDBO> CreateAsync(UserDBO user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<UserDBO?> GetByIdAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT \"Id\", \"Name\", \"Email\", \"PasswordHash\" FROM \"Users\" WHERE \"Id\" = @Id", connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserDBO
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3)
                };
            }
            return null;
        }

        public async Task<UserDBO?> GetByEmailAsync(string email)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT \"Id\", \"Name\", \"Email\", \"PasswordHash\" FROM \"Users\" WHERE \"Email\" = @Email", connection);
            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserDBO
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3)
                };
            }
            return null;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT COUNT(1) FROM \"Users\" WHERE \"Email\" = @Email", connection);
            cmd.Parameters.AddWithValue("@Email", email);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<UserDBO> CreateAsync(UserDBO user)
        {
            user.Id = Guid.NewGuid();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO \"Users\" (\"Id\", \"Name\", \"Email\", \"PasswordHash\") VALUES (@Id, @Name, @Email, @PasswordHash)",
                connection);
            
            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            await cmd.ExecuteNonQueryAsync();
            return user;
        }
    }
}
