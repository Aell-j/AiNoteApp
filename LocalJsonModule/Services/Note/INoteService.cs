using LocalJsonModule.DTOs.Notes;
using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public interface INoteService
{
    Task<List<Note>> GetAllAsync();
    Task<Note?> GetByIdAsync(Guid id);
    Task<List<Note>> GetByOwnerIdAsync(Guid ownerId);
    Task<List<Note>> GetByFolderIdAsync(Guid folderId);
    Task<Note> CreateAsync(CreateNoteRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateNoteRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteByOwnerIdAsync(Guid ownerId);
    Task<bool> DeleteByFolderIdAsync(Guid folderId);
    Task<bool> EnableToFavoriteAsync(Guid note_id);
    Task<bool> EnableToArchiveAsync(Guid note_id);
    Task<bool> DisableToArchiveAsync(Guid note_id);
    Task<int> DeleteAtArchiveNote();
}
