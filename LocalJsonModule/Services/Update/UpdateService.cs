using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Repositories;
using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public class UpdateService : IUpdateService
{
    public readonly IUserRepository _userRepository;
    //passwordHash
    
    public UpdateService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        //PassHashModule
    }
    
    public async Task<bool> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        if (request == null)
        {
            throw new ArgumentException("Данные пользователя не могут быть null.");
        }

        if (request.Login == null || request.Email == null || request.Password == null)
        {
            throw new ArgumentException("Логин, email и пароль не могут быть null.");
        }

        if (request.Login == "" || request.Email == "" || request.Password == "")
        {
            throw new ArgumentException("Логин, email и пароль не могут быть пустыми.");
        }

        User? user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return false;
        }

        User? existingLogin = await _userRepository.GetByLoginAsync(request.Login);

        if (existingLogin != null && existingLogin.Id != id)
        {
            throw new InvalidOperationException("Пользователь с таким логином уже существует.");
        }

        User? existingEmail = await _userRepository.GetByEmailAsync(request.Email);

        if (existingEmail != null && existingEmail.Id != id)
        {
            throw new InvalidOperationException("Пользователь с таким email уже существует.");
        }

        user.Login = request.Login;
        user.Email = request.Email;
        user.Password = request.Password;

        await _userRepository.UpdateAsync(user);
        return true;
    }
}