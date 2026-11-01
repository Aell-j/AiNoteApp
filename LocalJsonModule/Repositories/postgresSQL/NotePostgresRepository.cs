using LocalJsonModule.Data;
using LocalJsonModule.Models;
using Npgsql;
using NpgsqlTypes;

namespace LocalJsonModule.Repositories.postgresSQL;

public class NotePostgresRepository : INoteRepository
{
    private readonly IDBConnectionFactory _connectionString;
    

    public NotePostgresRepository(IDBConnectionFactory connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Note>> GetAllAsync()
    {
        const string query = "SELECT * FROM aelina.get_all_notes();";
        var notes = new List<Note>();

        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notes.Add(new Note
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Content = reader.GetString(reader.GetOrdinal("content")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                FolderId =  reader.GetGuid(reader.GetOrdinal("folder_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                IsFavorite = reader.GetBoolean(reader.GetOrdinal("is_favorite")),
                IsArchive = reader.GetBoolean(reader.GetOrdinal("is_archive")),
                Timer = reader.IsDBNull(reader.GetOrdinal("timer"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("timer")),
                NoteType = reader.IsDBNull(reader.GetOrdinal("note_type"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("note_type"))
            });
        }
        return notes;
    }

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        const string query = "SELECT * FROM aelina.get_all_by_id_note(@id);";
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Note
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Content = reader.GetString(reader.GetOrdinal("content")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                FolderId =  reader.GetGuid(reader.GetOrdinal("folder_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                IsFavorite = reader.GetBoolean(reader.GetOrdinal("is_favorite")),
                IsArchive = reader.GetBoolean(reader.GetOrdinal("is_archive")),
                Timer = reader.IsDBNull(reader.GetOrdinal("timer"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("timer")),
                NoteType = reader.IsDBNull(reader.GetOrdinal("note_type"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("note_type"))
            };
        }
        return null;
    }

    public async Task<List<Note>> GetByOwnerIdAsync(Guid ownerId)
    {
        const string query = "SELECT * FROM aelina.get_all_by_owner_id_note(@ownerId);";
        var notes = new List<Note>();
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@ownerId", ownerId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            notes.Add(new Note
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Content = reader.GetString(reader.GetOrdinal("content")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                FolderId =  reader.GetGuid(reader.GetOrdinal("folder_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                IsFavorite = reader.GetBoolean(reader.GetOrdinal("is_favorite")),
                IsArchive = reader.GetBoolean(reader.GetOrdinal("is_archive")),
                Timer = reader.IsDBNull(reader.GetOrdinal("timer"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("timer")),
                NoteType = reader.IsDBNull(reader.GetOrdinal("note_type"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("note_type"))
            });
        }
        return notes;
    }

    public async Task<List<Note>> GetByFolderIdAsync(Guid folderId)
    {
        const string query = "SELECT * FROM  aelina.get_all_by_folder_id_note(@folderId);";
        var notes = new List<Note>();
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@folderId", folderId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            notes.Add(new Note
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Content = reader.GetString(reader.GetOrdinal("content")),
                OwnerId = reader.GetGuid(reader.GetOrdinal("owner_id")),
                FolderId =  reader.GetGuid(reader.GetOrdinal("folder_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                IsFavorite = reader.GetBoolean(reader.GetOrdinal("is_favorite")),
                IsArchive = reader.GetBoolean(reader.GetOrdinal("is_archive")),
                Timer = reader.IsDBNull(reader.GetOrdinal("timer"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("timer")),
                NoteType = reader.IsDBNull(reader.GetOrdinal("note_type"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("note_type"))
            });
        }
        return notes;
    }

    public async Task AddAsync(Note note)
    {
        const string query = 
        "CALL aelina.add_note(@id, @title, @content, @owner_id, @folder_id, @created_at, @updated_at, @is_favorite, @is_archive, @timer, @note_type);"; 
        
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", note.Id);
        cmd.Parameters.AddWithValue("@title", note.Title);
        cmd.Parameters.AddWithValue("@content", note.Content);
        cmd.Parameters.AddWithValue("@owner_id", note.OwnerId);
        cmd.Parameters.AddWithValue("@folder_id", note.FolderId);
        cmd.Parameters.AddWithValue("@created_at", note.CreatedAt);
        cmd.Parameters.AddWithValue("@updated_at", note.UpdatedAt);
        cmd.Parameters.AddWithValue("@is_favorite", note.IsFavorite);
        cmd.Parameters.AddWithValue("@is_archive", note.IsArchive);
        cmd.Parameters.AddWithValue("@timer", NpgsqlDbType.TimestampTz, note.Timer ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@note_type", NpgsqlDbType.Varchar, note.NoteType ?? (object)DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Note note)
    {
        const string query = "CALL aelina.update_note(@id, @title, @content, @folder_id, @updated_at, @is_favorite, @is_archive, @note_type);";
            
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", note.Id);
        cmd.Parameters.AddWithValue("@title", note.Title);
        cmd.Parameters.AddWithValue("@content", note.Content);
        cmd.Parameters.Add(new NpgsqlParameter("@folder_id", NpgsqlDbType.Uuid)
            { Value = (object?)note.FolderId ?? DBNull.Value });
        cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@is_favorite", note.IsFavorite);
        cmd.Parameters.AddWithValue("@is_archive", note.IsArchive);
        cmd.Parameters.Add(new NpgsqlParameter("@note_type", NpgsqlDbType.Varchar)
            { Value = (object?)note.NoteType ?? DBNull.Value });
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        const string query = "CALL aelina.delete_note(@id);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteByOwnerIdAsync(Guid ownerId)
    {
        const string query = "CALL  aelina.delete_note_owner_id(@owner_id);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@owner_id", ownerId);
        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task DeleteByFolderIdAsync(Guid folderId)
    {
        const string query = "CALL aelina.delete_note_folder_id(@folder_id);";
        await using var connection = _connectionString.CreateConnection();
        await connection.OpenAsync();
        
        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@folder_id", folderId);
        await cmd.ExecuteNonQueryAsync();
    }
}