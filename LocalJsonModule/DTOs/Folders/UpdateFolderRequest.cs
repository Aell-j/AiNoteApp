namespace LocalJsonModule.DTOs.Folders;

public class UpdateFolderRequest
{
    public string Title { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
}
