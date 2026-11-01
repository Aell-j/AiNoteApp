using Npgsql;

namespace LocalJsonModule.Data;

public interface IDBConnectionFactory
{
    public NpgsqlConnection CreateConnection();
}