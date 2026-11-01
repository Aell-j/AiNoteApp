using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(AuthUserRequest aur);
}