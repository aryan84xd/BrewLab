using BrewLab.Data;
using BrewLab.Models.DBO;
using Npgsql;

namespace BrewLab.Repositories
{
    public interface IExperimentRepository
    {
        Task<ExperimentDBO?> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<ExperimentDBO>> GetByCoffeeIdAsync(Guid coffeeId, Guid userId);
        Task<ExperimentDBO> CreateAsync(ExperimentDBO experiment);
    }

    public class ExperimentRepository : IExperimentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ExperimentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ExperimentDBO?> GetByIdAsync(Guid id, Guid userId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                @"SELECT ""Id"", ""Date"", ""BrewMethod"", ""CoffeeWeight"", ""WaterWeight"", ""BrewTime"", ""Remark"", ""Aroma"", ""Acidity"", ""Body"", ""Overall"", ""CoffeeId"", ""UserId"" 
                  FROM ""Experiments"" 
                  WHERE ""Id"" = @Id AND ""UserId"" = @UserId",
                connection);
            
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapExperiment(reader);
            }
            return null;
        }

        public async Task<IEnumerable<ExperimentDBO>> GetByCoffeeIdAsync(Guid coffeeId, Guid userId)
        {
            var experiments = new List<ExperimentDBO>();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                @"SELECT ""Id"", ""Date"", ""BrewMethod"", ""CoffeeWeight"", ""WaterWeight"", ""BrewTime"", ""Remark"", ""Aroma"", ""Acidity"", ""Body"", ""Overall"", ""CoffeeId"", ""UserId"" 
                  FROM ""Experiments"" 
                  WHERE ""CoffeeId"" = @CoffeeId AND ""UserId"" = @UserId",
                connection);
            
            cmd.Parameters.AddWithValue("@CoffeeId", coffeeId);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                experiments.Add(MapExperiment(reader));
            }
            return experiments;
        }

        public async Task<ExperimentDBO> CreateAsync(ExperimentDBO experiment)
        {
            experiment.Id = Guid.NewGuid();
            experiment.Date = DateTime.UtcNow;

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var cmd = new NpgsqlCommand(
                @"INSERT INTO ""Experiments"" (""Id"", ""Date"", ""BrewMethod"", ""CoffeeWeight"", ""WaterWeight"", ""BrewTime"", ""Remark"", ""Aroma"", ""Acidity"", ""Body"", ""Overall"", ""CoffeeId"", ""UserId"")
                  VALUES (@Id, @Date, @BrewMethod, @CoffeeWeight, @WaterWeight, @BrewTime, @Remark, @Aroma, @Acidity, @Body, @Overall, @CoffeeId, @UserId)",
                connection);
            
            cmd.Parameters.AddWithValue("@Id", experiment.Id);
            cmd.Parameters.AddWithValue("@Date", experiment.Date);
            cmd.Parameters.AddWithValue("@BrewMethod", (object?)experiment.BrewMethod ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CoffeeWeight", experiment.CoffeeWeight);
            cmd.Parameters.AddWithValue("@WaterWeight", experiment.WaterWeight);
            cmd.Parameters.AddWithValue("@BrewTime", experiment.BrewTime);
            cmd.Parameters.AddWithValue("@Remark", (object?)experiment.Remark ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Aroma", experiment.Aroma);
            cmd.Parameters.AddWithValue("@Acidity", experiment.Acidity);
            cmd.Parameters.AddWithValue("@Body", experiment.Body);
            cmd.Parameters.AddWithValue("@Overall", experiment.Overall);
            cmd.Parameters.AddWithValue("@CoffeeId", experiment.CoffeeId);
            cmd.Parameters.AddWithValue("@UserId", experiment.UserId);

            await cmd.ExecuteNonQueryAsync();
            return experiment;
        }

        private ExperimentDBO MapExperiment(NpgsqlDataReader reader)
        {
            return new ExperimentDBO
            {
                Id = reader.GetGuid(0),
                Date = reader.GetDateTime(1),
                BrewMethod = reader.IsDBNull(2) ? null : reader.GetString(2),
                CoffeeWeight = reader.GetDecimal(3),
                WaterWeight = reader.GetDecimal(4),
                BrewTime = reader.GetFieldValue<TimeOnly>(5),
                Remark = reader.IsDBNull(6) ? null : reader.GetString(6),
                Aroma = reader.GetInt32(7),
                Acidity = reader.GetInt32(8),
                Body = reader.GetInt32(9),
                Overall = reader.GetInt32(10),
                CoffeeId = reader.GetGuid(11),
                UserId = reader.GetGuid(12)
            };
        }
    }
}
