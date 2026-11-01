using LocalJsonModule.Models;

namespace LocalJsonModule.Repositories;

public interface INoteRepository
{
    Task<List<Note>> GetAllAsync();
    Task<Note?> GetByIdAsync(Guid id);
    Task<List<Note>> GetByOwnerIdAsync(Guid ownerId);
    Task<List<Note>> GetByFolderIdAsync(Guid folderId);
    Task AddAsync(Note note);
    Task UpdateAsync(Note note);
    Task DeleteAsync(Guid id);
    Task DeleteByOwnerIdAsync(Guid ownerId);
    Task DeleteByFolderIdAsync(Guid folderId);
}
