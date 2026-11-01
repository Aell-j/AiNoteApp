using LocalJsonModule.DTOs.Folders;
using LocalJsonModule.Models;
using LocalJsonModule.Repositories;

namespace LocalJsonModule.Services;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly INoteRepository _noteRepository;

    public FolderService(
        IFolderRepository repository,
        IUserRepository userRepository,
        INoteRepository noteRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
        _noteRepository = noteRepository;
    }

    public async Task<List<Folder>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Folder?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Folder>> GetByOwnerIdAsync(Guid ownerId)
    {
        if (ownerId == Guid.Empty)
        {
            throw new KeyNotFoundException("Айди пользователя не может быть пустым");
        }

        var owner = await _userRepository.GetByIdAsync(ownerId);
        if (owner == null)
        {
            throw new KeyNotFoundException($"Владелец {ownerId} не найден");
        }

        return await _repository.GetByOwnerIdAsync(ownerId);
    }

    public async Task<List<Folder>> GetByParentFolderIdAsync(Guid ownerId, Guid? parentFolderId)
    {
        return await _repository.GetByParentFolderIdAsync(ownerId, parentFolderId);
    }

    public async Task<Folder> CreateAsync(CreateFolderRequest request)
    {
        if (request == null)
        {
            throw new ArgumentException("Данные папки не могут быть null.");
        }

        if (request.Title == null)
        {
            throw new ArgumentException("Название папки не может быть null.");
        }

        if (request.Title == "")
        {
            throw new ArgumentException("Название папки не может быть пустым.");
        }

        User? owner = await _userRepository.GetByIdAsync(request.OwnerId);

        if (owner == null)
        {
            throw new KeyNotFoundException("Владелец папки не найден.");
        }

        if (request.ParentFolderId != null)
        {
            Folder? parentFolder = await _repository.GetByIdAsync(request.ParentFolderId.Value);

            if (parentFolder == null)
            {
                throw new KeyNotFoundException("Родительская папка не найдена.");
            }

            if (parentFolder.OwnerId != request.OwnerId)
            {
                throw new InvalidOperationException("Родительская папка принадлежит другому пользователю.");
            }
        }

        Folder folder = new Folder
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            OwnerId = request.OwnerId,
            ParentFolderId = request.ParentFolderId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(folder);
        return folder;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateFolderRequest request)
    {
        if (request == null)
        {
            throw new ArgumentException("Данные папки не могут быть null.");
        }

        if (request.Title == null)
        {
            throw new ArgumentException("Название папки не может быть null.");
        }

        if (request.Title == "")
        {
            throw new ArgumentException("Название папки не может быть пустым.");
        }

        Folder? folder = await _repository.GetByIdAsync(id);

        if (folder == null)
        {
            return false;
        }

        if (request.ParentFolderId == id)
        {
            throw new InvalidOperationException("Папка не может быть родителем самой себя.");
        }

        if (request.ParentFolderId != null)
        {
            Folder? parentFolder = await _repository.GetByIdAsync(request.ParentFolderId.Value);

            if (parentFolder == null)
            {
                throw new KeyNotFoundException("Родительская папка не найдена.");
            }

            if (parentFolder.OwnerId != folder.OwnerId)
            {
                throw new InvalidOperationException("Родительская папка принадлежит другому пользователю.");
            }
        }

        folder.Title = request.Title;
        folder.ParentFolderId = request.ParentFolderId;

        await _repository.UpdateAsync(folder);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        Folder? folder = await _repository.GetByIdAsync(id);

        if (folder == null)
        {
            return false;
        }

        List<Folder> childFolders = await _repository.GetByParentFolderIdAsync(folder.OwnerId, id);

        if (childFolders.Count > 0)
        {
            throw new InvalidOperationException("Нельзя удалить папку, у которой есть вложенные папки.");
        }

        List<Note> notes = await _noteRepository.GetByFolderIdAsync(id);

        if (notes.Count > 0)
        {
            throw new InvalidOperationException("Нельзя удалить папку, в которой есть заметки.");
        }

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> DeleteByOwnerIdAsync(Guid ownerId)
    {
        List<Folder> folders = await _repository.GetByOwnerIdAsync(ownerId);

        if (folders.Count == 0)
        {
            return false;
        }

        await _noteRepository.DeleteByOwnerIdAsync(ownerId);
        await _repository.DeleteByOwnerIdAsync(ownerId);
        return true;
    }
}
