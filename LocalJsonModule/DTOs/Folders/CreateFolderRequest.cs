namespace LocalJsonModule.DTOs.Folders;

public class CreateFolderRequest
{
    public string Title { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; } = null;
    public Guid OwnerId { get; set; }
}
