using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;
using LocalJsonModule.Repositories;

namespace LocalJsonModule.Services;

public class RegisterService : IRegisterService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    
    public RegisterService(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<User?> RegisterAsync(RegisterUserRequest rur)
    {
        if (rur == null)
        {
            throw new ArgumentException("Данные пользователя не могут быть null.");
        }

        if (string.IsNullOrWhiteSpace(rur.Login) || string.IsNullOrWhiteSpace(rur.Password) ||
            string.IsNullOrWhiteSpace(rur.Email))
        {
            throw new ArgumentException("Логин, email и пароль не могут быть null.");
        }

        if (rur.Login == "" || rur.Email == "" || rur.Password == "")
        {
            throw new ArgumentException("Логин, email и пароль не могут быть пустыми.");
        }

        string passwordHash = await _passwordService.HashPass(rur.Password);
        
        User? existingLogin = await _userRepository.GetByLoginAsync(rur.Login);

        if (existingLogin != null)
        {
            throw new InvalidOperationException("Пользователь с таким логином уже существует.");
        }

        User? existingEmail = await _userRepository.GetByEmailAsync(rur.Email);

        if (existingEmail != null)
        {
            throw new InvalidOperationException("Пользователь с таким email уже существует.");
        }

        User user = new User()
        {
            Id = Guid.NewGuid(),
            Login = rur.Login,
            Email = rur.Email,
            Password = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
        await _userRepository.AddAsync(user);
        return user;
    }
}