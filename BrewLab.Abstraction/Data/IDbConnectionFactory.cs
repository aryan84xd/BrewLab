using Npgsql;

namespace BrewLab.Abstraction.Data
{
    public interface IDbConnectionFactory
    {
        Task<NpgsqlConnection> CreateConnectionAsync();
    }
}
