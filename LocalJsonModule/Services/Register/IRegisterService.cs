using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public interface IRegisterService
{
    Task<User?> RegisterAsync(RegisterUserRequest rur);
}