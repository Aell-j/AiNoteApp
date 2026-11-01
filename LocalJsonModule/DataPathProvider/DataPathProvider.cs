namespace LocalJsonModule.DataPathProvider;

public class DataPathProvider : IDataPathProvider
{
    private readonly string _dataDirectory;

    public DataPathProvider()
    {
        _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(_dataDirectory);
    }

    public string GetUsersFilePath()
    {
        return Path.Combine(_dataDirectory, "users.json");
    }

    public string GetNotesFilePath()
    {
        return Path.Combine(_dataDirectory, "notes.json");
    }
}
