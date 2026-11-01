using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public interface IUpdateService
{
    Task<bool> UpdateAsync(Guid id, UpdateUserRequest request);
}