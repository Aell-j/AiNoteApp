using LocalJsonModule.DTOs.Folders;
using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public interface IFolderService
{
    Task<List<Folder>> GetAllAsync();
    Task<Folder?> GetByIdAsync(Guid id);
    Task<List<Folder>> GetByOwnerIdAsync(Guid ownerId);
    Task<List<Folder>> GetByParentFolderIdAsync(Guid ownerId, Guid? parentFolderId);
    Task<Folder> CreateAsync(CreateFolderRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateFolderRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteByOwnerIdAsync(Guid ownerId);
}
