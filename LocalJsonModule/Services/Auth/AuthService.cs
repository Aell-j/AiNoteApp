using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;
using LocalJsonModule.Repositories;

namespace LocalJsonModule.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    
    public AuthService(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<User?> LoginAsync(AuthUserRequest aur)
    {
        if (aur == null || 
            string.IsNullOrWhiteSpace(aur.Login) || 
            string.IsNullOrWhiteSpace(aur.Password))
        {
            throw new ArgumentException("Логин и пароль обязательны");
        }
        
        User? user = await _userRepository.GetByLoginAsync(aur.Login);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Неверный логин или пароль");
        }
        bool passwordCorrect = _passwordService.VerifyPass(user.Password, aur.Password);
        if (!passwordCorrect)
        {
            throw new UnauthorizedAccessException("Неверный логин или пароль");
        }
        return user;
    }
}