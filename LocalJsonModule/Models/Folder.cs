namespace LocalJsonModule.Models;

public class Folder
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public Guid? ParentFolderId { get; set; } = null;
    public DateTime CreatedAt { get; set; }
}
