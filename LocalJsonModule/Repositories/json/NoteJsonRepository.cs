using System.Text.Json;
using LocalJsonModule.Data;
using LocalJsonModule.Models;

namespace LocalJsonModule.Repositories;

public class NoteJsonRepository : INoteRepository
{
    private readonly IDataPathProvider _dataPathProvider;

    public NoteJsonRepository(IDataPathProvider dataPathProvider)
    {
        _dataPathProvider = dataPathProvider;
    }

    public async Task<List<Note>> GetAllAsync()
    {
        try
        {
            string filePath = _dataPathProvider.GetNotesFilePath();

            if (!File.Exists(filePath))
            {
                return new List<Note>();
            }

            string json = await File.ReadAllTextAsync(filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Note>();
            }

            return JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Не удалось прочитать заметки из JSON.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Не удалось прочитать файл заметок.", ex);
        }
    }

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        List<Note> notes = await GetAllAsync();
        return notes.FirstOrDefault(n => n.Id == id);
    }

    public async Task<List<Note>> GetByOwnerIdAsync(Guid ownerId)
    {
        List<Note> notes = await GetAllAsync();
        return notes.Where(n => n.OwnerId == ownerId).ToList();
    }

    public async Task<List<Note>> GetByFolderIdAsync(Guid folderId)
    {
        List<Note> notes = await GetAllAsync();
        return notes.Where(n => n.FolderId == folderId).ToList();
    }

    public async Task AddAsync(Note note)
    {
        List<Note> notes = await GetAllAsync();
        notes.Add(note);
        await SaveAllAsync(notes);
    }

    public async Task UpdateAsync(Note note)
    {
        List<Note> notes = await GetAllAsync();
        Note? noteToUpdate = notes.FirstOrDefault(n => n.Id == note.Id);

        if (noteToUpdate == null)
        {
            throw new KeyNotFoundException("Заметка не найдена.");
        }

        noteToUpdate.Title = note.Title;
        noteToUpdate.Content = note.Content;
        noteToUpdate.FolderId = note.FolderId;
        noteToUpdate.UpdatedAt = note.UpdatedAt;
        noteToUpdate.IsFavorite = note.IsFavorite;
        noteToUpdate.IsArchive = note.IsArchive;
        noteToUpdate.Timer = note.Timer;
        noteToUpdate.NoteType = note.NoteType;

        await SaveAllAsync(notes);
    }

    public async Task DeleteAsync(Guid id)
    {
        List<Note> notes = await GetAllAsync();
        Note? note = notes.FirstOrDefault(n => n.Id == id);

        if (note == null)
        {
            return;
        }

        notes.Remove(note);
        await SaveAllAsync(notes);
    }

    public async Task DeleteByOwnerIdAsync(Guid ownerId)
    {
        List<Note> notes = await GetAllAsync();
        notes.RemoveAll(n => n.OwnerId == ownerId);
        await SaveAllAsync(notes);
    }

    public async Task DeleteByFolderIdAsync(Guid folderId)
    {
        List<Note> notes = await GetAllAsync();
        notes.RemoveAll(n => n.FolderId == folderId);
        await SaveAllAsync(notes);
    }

    private async Task SaveAllAsync(List<Note> notes)
    {
        try
        {
            string filePath = _dataPathProvider.GetNotesFilePath();
            string json = JsonSerializer.Serialize(notes, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Не удалось записать файл заметок.", ex);
        }
    }
}
