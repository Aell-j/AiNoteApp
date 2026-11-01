using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;
using LocalJsonModule.Repositories;

namespace LocalJsonModule.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly INoteRepository _noteRepository;
    private readonly IFolderRepository _folderRepository;

    public UserService(
        IUserRepository repository,
        INoteRepository noteRepository,
        IFolderRepository folderRepository)
    {
        _repository = repository;
        _noteRepository = noteRepository;
        _folderRepository = folderRepository;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        if (login == null)
        {
            throw new ArgumentException("Логин не может быть null.");
        }

        return await _repository.GetByLoginAsync(login);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (email == null)
        {
            throw new ArgumentException("Email не может быть null.");
        }

        return await _repository.GetByEmailAsync(email);
    }

    

    public async Task<bool> DeleteAsync(Guid id)
    {
        User? user = await _repository.GetByIdAsync(id);

        if (user == null)
        {
            return false;
        }

        await _noteRepository.DeleteByOwnerIdAsync(id);
        await _folderRepository.DeleteByOwnerIdAsync(id);
        await _repository.DeleteAsync(id);
        
        return true;
    }
}
