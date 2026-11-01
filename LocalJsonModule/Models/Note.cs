namespace LocalJsonModule.Models;

public class Note
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public Guid? FolderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsArchive { get; set; }
    public DateTime? Timer { get; set; }
    public string? NoteType { get; set; }
}
