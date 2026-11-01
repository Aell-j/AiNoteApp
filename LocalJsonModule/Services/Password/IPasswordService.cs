namespace LocalJsonModule.Services;

public interface IPasswordService
{
    Task<string> HashPass(string userPass);
    bool VerifyPass(string userPass, string hashedPass);
}