using BrewLab.Abstraction.Data;
using BrewLab.Abstraction.Repositories;
using BrewLab.DomainModel.DBModels;
using Npgsql;

namespace BrewLab.DataAccess.Repositories
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CoffeeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CoffeeDBO?> GetByIdAsync(Guid id, Guid userId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT \"Id\", \"Name\", \"Brand\", \"Roast\", \"Origin\", \"TastingNotes\", \"UserId\" FROM \"Coffees\" WHERE \"Id\" = @Id AND \"UserId\" = @UserId",
                connection);
            
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CoffeeDBO
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Brand = reader.GetString(2),
                    Roast = reader.GetString(3),
                    Origin = reader.IsDBNull(4) ? null : reader.GetString(4),
                    TastingNotes = reader.IsDBNull(5) ? null : reader.GetString(5),
                    UserId = reader.GetGuid(6)
                };
            }
            return null;
        }

        public async Task<IEnumerable<CoffeeDBO>> GetAllByUserIdAsync(Guid userId)
        {
            var coffees = new List<CoffeeDBO>();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT \"Id\", \"Name\", \"Brand\", \"Roast\", \"Origin\", \"TastingNotes\", \"UserId\" FROM \"Coffees\" WHERE \"UserId\" = @UserId",
                connection);
            
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                coffees.Add(new CoffeeDBO
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Brand = reader.GetString(2),
                    Roast = reader.GetString(3),
                    Origin = reader.IsDBNull(4) ? null : reader.GetString(4),
                    TastingNotes = reader.IsDBNull(5) ? null : reader.GetString(5),
                    UserId = reader.GetGuid(6)
                });
            }
            return coffees;
        }

        public async Task<CoffeeDBO> CreateAsync(CoffeeDBO coffee)
        {
            coffee.Id = Guid.NewGuid();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO \"Coffees\" (\"Id\", \"Name\", \"Brand\", \"Roast\", \"Origin\", \"TastingNotes\", \"UserId\") VALUES (@Id, @Name, @Brand, @Roast, @Origin, @TastingNotes, @UserId)",
                connection);
            
            cmd.Parameters.AddWithValue("@Id", coffee.Id);
            cmd.Parameters.AddWithValue("@Name", coffee.Name);
            cmd.Parameters.AddWithValue("@Brand", coffee.Brand);
            cmd.Parameters.AddWithValue("@Roast", coffee.Roast);
            cmd.Parameters.AddWithValue("@Origin", (object?)coffee.Origin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TastingNotes", (object?)coffee.TastingNotes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserId", coffee.UserId);

            await cmd.ExecuteNonQueryAsync();
            return coffee;
        }

        public async Task<bool> ExistsAsync(Guid id, Guid userId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT COUNT(1) FROM \"Coffees\" WHERE \"Id\" = @Id AND \"UserId\" = @UserId",
                connection);
            
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
    }
}
