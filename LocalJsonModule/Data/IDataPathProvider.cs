namespace LocalJsonModule.Data;

public interface IDataPathProvider
{
    string GetUsersFilePath();
    string GetFoldersFilePath();
    string GetNotesFilePath();
}
