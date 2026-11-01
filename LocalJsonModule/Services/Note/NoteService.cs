using LocalJsonModule.DTOs.Notes;
using LocalJsonModule.Models;
using LocalJsonModule.Repositories;

namespace LocalJsonModule.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IFolderRepository _folderRepository;

    public NoteService(
        INoteRepository repository,
        IUserRepository userRepository,
        IFolderRepository folderRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
        _folderRepository = folderRepository;
    }

    public async Task<List<Note>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Note>> GetByOwnerIdAsync(Guid ownerId)
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

    public async Task<List<Note>> GetByFolderIdAsync(Guid folderId)
    {
        if (folderId == Guid.Empty)
        {
            throw new KeyNotFoundException("Айди папки не может быть пустым");
        }
        var folder = await _folderRepository.GetByIdAsync(folderId);
        if (folder == null)
        {
            throw new KeyNotFoundException("Папка не найдена");
        }
        return await _repository.GetByFolderIdAsync(folderId);
    }

    public async Task<Note> CreateAsync(CreateNoteRequest request)
    {
        if (request == null)
        {
            throw new ArgumentException("Данные заметки не могут быть null.");
        }

        if (request.Title == null || request.Content == null)
        {
            throw new ArgumentException("Название и содержимое заметки не могут быть null.");
        }

        if (request.Title == "")
        {
            throw new ArgumentException("Название не может быть пустым.");
        }

        User? owner = await _userRepository.GetByIdAsync(request.OwnerId);

        if (owner == null)
        {
            throw new KeyNotFoundException("Владелец заметки не найден.");
        }

        if (request.FolderId != null)
        {
            Folder? folder = await _folderRepository.GetByIdAsync(request.FolderId.Value);

            if (folder == null)
            {
                throw new KeyNotFoundException("Папка не найдена.");
            }

            if (folder.OwnerId != request.OwnerId)
            {
                throw new InvalidOperationException("Папка принадлежит другому пользователю.");
            }
        }

        Note note = new Note
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            OwnerId = request.OwnerId,
            FolderId = request.FolderId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsFavorite = request.IsFavorite,
            IsArchive = request.IsArchive,
            NoteType = request.NoteType
        };

        await _repository.AddAsync(note);
        return note;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateNoteRequest request)
    {
        if (request == null)
        {
            throw new ArgumentException("Данные заметки не могут быть null.");
        }

        if (request.Title == null || request.Content == null)
        {
            throw new ArgumentException("Название и содержимое заметки не могут быть null.");
        }

        if (request.Title == "")
        {
            throw new ArgumentException("Название не может быть пустым.");
        }

        Note? note = await _repository.GetByIdAsync(id);

        if (note == null)
        {
            return false;
        }

        if (request.FolderId != null)
        {
            Folder? folder = await _folderRepository.GetByIdAsync(request.FolderId.Value);

            if (folder == null)
            {
                throw new KeyNotFoundException("Папка не найдена.");
            }

            if (folder.OwnerId != note.OwnerId)
            {
                throw new InvalidOperationException("Папка принадлежит другому пользователю.");
            }
        }

        note.Title = request.Title;
        note.Content = request.Content;
        note.FolderId = request.FolderId;
        note.UpdatedAt = DateTime.UtcNow;
        note.IsFavorite = request.IsFavorite;
        note.IsArchive = request.IsArchive;
        note.NoteType = request.NoteType;

        await _repository.UpdateAsync(note);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        Note? note = await _repository.GetByIdAsync(id);

        if (note == null)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> DeleteByOwnerIdAsync(Guid ownerId)
    {
        List<Note> notes = await _repository.GetByOwnerIdAsync(ownerId);

        if (notes.Count == 0)
        {
            return false;
        }

        await _repository.DeleteByOwnerIdAsync(ownerId);
        return true;
    }

    public async Task<bool> DeleteByFolderIdAsync(Guid folderId)
    {
        List<Note> notes = await _repository.GetByFolderIdAsync(folderId);

        if (notes.Count == 0)
        {
            return false;
        }

        await _repository.DeleteByFolderIdAsync(folderId);
        return true;
    }

    public async Task<bool> EnableToFavoriteAsync(Guid note_id)
    {
        Note? note = await _repository.GetByIdAsync(note_id);
        if (note == null)
        {
            return false;
        }
        note.IsFavorite = !note.IsFavorite;
        note.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(note);
        return true;
    }

    public async Task<bool> EnableToArchiveAsync(Guid note_id)
    {
        Note? note = await _repository.GetByIdAsync(note_id);
        if (note == null)
        {
            return false;
        }

        note.IsArchive = true;
        note.UpdatedAt = DateTime.UtcNow;
        note.Timer = DateTime.UtcNow.AddDays(7);
        await _repository.UpdateAsync(note);
        return true;
    }

    public async Task<bool> DisableToArchiveAsync(Guid note_id)
    {
        Note? note = await _repository.GetByIdAsync(note_id);
        if (note == null)
        {
            return false;
        }
        note.IsArchive =  false;
        note.UpdatedAt = DateTime.UtcNow;
        note.Timer = null;
        await _repository.UpdateAsync(note);
        return true;
    }

    public async Task<int> DeleteAtArchiveNote()
    {
        List<Note> notes = await GetAllAsync();
        DateTime now = DateTime.UtcNow;

        List<Note> noteTimerExpired = notes.Where(n => n.IsArchive && n.Timer.HasValue && n.Timer >= now).ToList();
        foreach (Note n in noteTimerExpired)
        {
            await  _repository.DeleteAsync(n.Id);
        }

        return noteTimerExpired.Count;
    }
}
