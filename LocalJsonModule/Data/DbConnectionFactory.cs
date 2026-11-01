using Npgsql;

namespace LocalJsonModule.Data;

public class DbConnectionFactory : IDBConnectionFactory
{
    private readonly string _connectionString;
    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}