using LocalJsonModule.Data;
using LocalJsonModule.Models;
using Npgsql;
using NpgsqlTypes;

namespace LocalJsonModule.Repositories.postgresSQL;

public class FolderPostgresRepository : IFolderRepository
{
    private readonly IDBConnectionFactory _connectionString;
    

    public FolderPostgresRepository(IDBConnectionFactory connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Folder>> GetAllAsync()
    {
        const string query = "SELECT * FROM aelina.get_all_folders();";
        var folders = new List<Folder>();

        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            folders.Add(new Folder
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ParentFolderId = reader.IsDBNull(reader.GetOrdinal("parent_folder_id"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("parent_folder_id")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            });
        }
        return folders;
    }

    public async Task<Folder?> GetByIdAsync(Guid id)
    {
        const string query = "SELECT * FROM aelina.get_by_folder_id(@id);";
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Folder
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ParentFolderId = reader.IsDBNull(reader.GetOrdinal("parent_folder_id"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("parent_folder_id")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            };
        }
        return null;
    }

    public async Task<List<Folder>> GetByOwnerIdAsync(Guid ownerId)
    {
        const string query = "SELECT * FROM aelina.get_by_owner_id(@ownerId);";
        var folders = new List<Folder>();
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@ownerId", ownerId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            folders.Add(new Folder
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ParentFolderId = reader.IsDBNull(reader.GetOrdinal("parent_folder_id"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("parent_folder_id")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            });
        }
        return folders;
    }

    public async Task<List<Folder>> GetByParentFolderIdAsync(Guid ownerId, Guid? parentFolderId)
    {
        const string query = "SELECT * FROM aelina.get_by_parent_folder_id(@ownerId, @parentFolderId);";
        var folders = new List<Folder>();
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@parentFolderId", parentFolderId);
        cmd.Parameters.AddWithValue("@ownerId", ownerId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            folders.Add(new Folder
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ParentFolderId = reader.IsDBNull(reader.GetOrdinal("parent_folder_id"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("parent_folder_id")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            });
        }
        return folders;
    }

    public async Task AddAsync(Folder folder)
    {
        const string query =
            "CALL aelina.add_folder(@id, @title, @owner_id, @parent_folder_id, @created_at);";

        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(query, connection);

        cmd.Parameters.AddWithValue("@id", folder.Id);
        cmd.Parameters.AddWithValue("@title", folder.Title);
        cmd.Parameters.AddWithValue("@owner_id", NpgsqlDbType.Uuid, folder.OwnerId);
        cmd.Parameters.AddWithValue(
            "@parent_folder_id",
            NpgsqlDbType.Uuid,
            folder.ParentFolderId.HasValue
                ? folder.ParentFolderId.Value
                : DBNull.Value
        );
        cmd.Parameters.AddWithValue("@created_at", folder.CreatedAt);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Folder folder)
    {
        const string query = "CALL aelina.update_folder(@id, @title, @parent_folder_id);"; 
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        
        cmd.Parameters.AddWithValue("@id", folder.Id);
        cmd.Parameters.AddWithValue("@title", folder.Title);
        cmd.Parameters.AddWithValue(
            "@parent_folder_id",
            NpgsqlDbType.Uuid,
            folder.ParentFolderId.HasValue
                ? folder.ParentFolderId.Value
                : DBNull.Value
        );
        cmd.Parameters.AddWithValue("@created_at", folder.CreatedAt);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        const string query = "CALL aelina.delete_folder(@id);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteByOwnerIdAsync(Guid ownerId)
    {
        const string query = "CALL aelina.delete_folder_by_owner_id(@ownerId);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@ownerId", ownerId);
        await cmd.ExecuteNonQueryAsync();
    }
}