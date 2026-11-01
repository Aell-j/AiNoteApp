using System.Text.Json;
using LocalJsonModule.Data;
using LocalJsonModule.Models;

namespace LocalJsonModule.Repositories;

public class FolderJsonRepository : IFolderRepository
{
    private readonly IDataPathProvider _dataPathProvider;

    public FolderJsonRepository(IDataPathProvider dataPathProvider)
    {
        _dataPathProvider = dataPathProvider;
    }

    public async Task<List<Folder>> GetAllAsync()
    {
        try
        {
            string filePath = _dataPathProvider.GetFoldersFilePath();

            if (!File.Exists(filePath))
            {
                return new List<Folder>();
            }

            string json = await File.ReadAllTextAsync(filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Folder>();
            }

            return JsonSerializer.Deserialize<List<Folder>>(json) ?? new List<Folder>();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Не удалось прочитать папки из JSON.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Не удалось прочитать файл папок.", ex);
        }
    }

    public async Task<Folder?> GetByIdAsync(Guid id)
    {
        List<Folder> folders = await GetAllAsync();
        return folders.FirstOrDefault(f => f.Id == id);
    }

    public async Task<List<Folder>> GetByOwnerIdAsync(Guid ownerId)
    {
        List<Folder> folders = await GetAllAsync();
        return folders.Where(f => f.OwnerId == ownerId).ToList();
    }

    public async Task<List<Folder>> GetByParentFolderIdAsync(Guid ownerId, Guid? parentFolderId)
    {
        List<Folder> folders = await GetAllAsync();
        return folders
            .Where(f => f.OwnerId == ownerId && f.ParentFolderId == parentFolderId)
            .ToList();
    }

    public async Task AddAsync(Folder folder)
    {
        List<Folder> folders = await GetAllAsync();
        folders.Add(folder);
        await SaveAllAsync(folders);
    }

    public async Task UpdateAsync(Folder folder)
    {
        List<Folder> folders = await GetAllAsync();
        Folder? folderToUpdate = folders.FirstOrDefault(f => f.Id == folder.Id);

        if (folderToUpdate == null)
        {
            throw new KeyNotFoundException("Папка не найдена.");
        }

        folderToUpdate.Title = folder.Title;
        folderToUpdate.ParentFolderId = folder.ParentFolderId;

        await SaveAllAsync(folders);
    }

    public async Task DeleteAsync(Guid id)
    {
        List<Folder> folders = await GetAllAsync();
        Folder? folder = folders.FirstOrDefault(f => f.Id == id);

        if (folder == null)
        {
            return;
        }

        folders.Remove(folder);
        await SaveAllAsync(folders);
    }

    public async Task DeleteByOwnerIdAsync(Guid ownerId)
    {
        List<Folder> folders = await GetAllAsync();
        folders.RemoveAll(f => f.OwnerId == ownerId);
        await SaveAllAsync(folders);
    }

    private async Task SaveAllAsync(List<Folder> folders)
    {
        try
        {
            string filePath = _dataPathProvider.GetFoldersFilePath();
            string json = JsonSerializer.Serialize(folders, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Не удалось записать файл папок.", ex);
        }
    }
}
