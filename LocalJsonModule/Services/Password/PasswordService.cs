using LocalJsonModule.Repositories;
using Microsoft.AspNetCore.Identity;

namespace LocalJsonModule.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher;

    public PasswordService()
    {
        _hasher = new PasswordHasher<object>();
    }
    public async Task<string> HashPass(string userPass)
    {
        if (string.IsNullOrWhiteSpace(userPass))
        {
            throw new ArgumentNullException("Пароль не должен быть пустым");
        }
        return _hasher.HashPassword(null!,  userPass);
    }

    public bool VerifyPass(string userPass, string hashedPass)
    {
        if(string.IsNullOrWhiteSpace(userPass) || (string.IsNullOrWhiteSpace(hashedPass)))
        {
            return false;
        }
        PasswordVerificationResult result = _hasher.VerifyHashedPassword(null!, hashedPass, userPass);
        return (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.Failed);
    }
}