using LocalJsonModule.Models;

namespace LocalJsonModule.Repositories;

public interface IFolderRepository
{
    Task<List<Folder>> GetAllAsync();
    Task<Folder?> GetByIdAsync(Guid id);
    Task<List<Folder>> GetByOwnerIdAsync(Guid ownerId);
    Task<List<Folder>> GetByParentFolderIdAsync(Guid ownerId, Guid? parentFolderId);
    Task AddAsync(Folder folder);
    Task UpdateAsync(Folder folder);
    Task DeleteAsync(Guid id);
    Task DeleteByOwnerIdAsync(Guid ownerId);
}
