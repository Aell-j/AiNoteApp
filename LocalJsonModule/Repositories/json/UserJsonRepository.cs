using System.Text.Json;
using LocalJsonModule.Data;
using LocalJsonModule.Models;

namespace LocalJsonModule.Repositories;

public class UserJsonRepository : IUserRepository
{
    private readonly IDataPathProvider _dataPathProvider;

    public UserJsonRepository(IDataPathProvider dataPathProvider)
    {
        _dataPathProvider = dataPathProvider;
    }

    public async Task<List<User>> GetAllAsync()
    {
        try
        {
            string filePath = _dataPathProvider.GetUsersFilePath();

            if (!File.Exists(filePath))
            {
                return new List<User>();
            }

            string json = await File.ReadAllTextAsync(filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<User>();
            }

            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Не удалось прочитать пользователей из JSON.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Не удалось прочитать файл пользователей.", ex);
        }
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        List<User> users = await GetAllAsync();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        List<User> users = await GetAllAsync();
        return users.FirstOrDefault(u => u.Login == login);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        List<User> users = await GetAllAsync();
        return users.FirstOrDefault(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        try
        {
            List<User> users = await GetAllAsync();
            users.Add(user);
            await SaveAllAsync(users);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Не удалось сохранить пользователя.", ex);
        }
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await GetAllAsync();
        User? userToUpdate = users.FirstOrDefault(u => u.Id == user.Id);

        if (userToUpdate == null)
        {
            throw new KeyNotFoundException("Пользователь не найден.");
        }

        userToUpdate.Login = user.Login;
        userToUpdate.Email = user.Email;
        userToUpdate.Password = user.Password;

        await SaveAllAsync(users);
    }

    public async Task DeleteAsync(Guid id)
    {
        List<User> users = await GetAllAsync();
        User? user = users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return;
        }

        users.Remove(user);
        await SaveAllAsync(users);
    }

    private async Task SaveAllAsync(List<User> users)
    {
        try
        {
            string filePath = _dataPathProvider.GetUsersFilePath();
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Не удалось записать файл пользователей.", ex);
        }
    }
}
