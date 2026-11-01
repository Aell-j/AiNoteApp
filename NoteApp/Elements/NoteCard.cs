using System;
using LocalJsonModule.Models;

namespace NoteApp;

/// Содержимое карточки заметки. Внешний вид живёт в DataTemplate в MainWindow.axaml.
internal sealed class NoteCard(Note note, string? folderTitle)
{
    public Note Note { get; } = note;

    public string Title => Note.Title;

    public string Preview => string.IsNullOrWhiteSpace(Note.Content)
        ? "Пустая заметка"
        : NoteFormat.Shorten(Note.Content, 120);

    public string Category => (Note.IsArchive
        ? "Корзина"
        : folderTitle ?? (Note.FolderId is null ? "Без папки" : "Папка")).ToUpperInvariant();

    public bool IsFavorite => Note.IsFavorite;

    public string DateText => NoteFormat.FormatDate(Note.UpdatedAt);
}
