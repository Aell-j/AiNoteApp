using LocalJsonModule.Data;
using LocalJsonModule.Models;
using Npgsql;

namespace LocalJsonModule.Repositories.postgresSQL;

public class UserPostgresRepository : IUserRepository
{
    private readonly IDBConnectionFactory _connectionString;
    

    public UserPostgresRepository(IDBConnectionFactory connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<User>> GetAllAsync()
    {
        const string query = "SELECT * FROM aelina.get_all_users();";
        var users = new List<User>();

        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Login = reader.GetString(reader.GetOrdinal("login")),
                Password = reader.GetString(reader.GetOrdinal("password_hash")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            });
        }
        return users;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string query = "SELECT * FROM aelina.get_by_id(@id);";
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Login = reader.GetString(reader.GetOrdinal("login")),
                Password = reader.GetString(reader.GetOrdinal("password_hash")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
        return null;
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        const string query = "SELECT * FROM aelina.get_by_login(@login)";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@login", login);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Login = reader.GetString(reader.GetOrdinal("login")),
                Password = reader.GetString(reader.GetOrdinal("password_hash")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
        return null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string query = "SELECT * FROM aelina.get_by_email(@email);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@email", email);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Login = reader.GetString(reader.GetOrdinal("login")),
                Password = reader.GetString(reader.GetOrdinal("password_hash")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
        return null;
    }

    public async Task AddAsync(User user)
    {
        const string query = "CALL aelina.add_user(@id, @login,  @email, @password_hash, @created_at);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", user.Id);
        cmd.Parameters.AddWithValue("@login", user.Login);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@password_hash", user.Password);
        cmd.Parameters.AddWithValue("@created_at", user.CreatedAt);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(User user)
    {
        const string query = "CALL aelina.update_user(@id, @login,  @email, @password_hash)";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", user.Id);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@login", user.Login);
        cmd.Parameters.AddWithValue("@password_hash", user.Password);
        
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        const string query = "CALL aelina.delete_user(@id);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}