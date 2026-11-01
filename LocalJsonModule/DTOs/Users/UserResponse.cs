namespace LocalJsonModule.DTOs.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserResponse(Guid id, string email, string login, DateTime createdAt)
    {
        Id = id;
        Email = email;
        Login = login;
        CreatedAt = createdAt;
    }
}