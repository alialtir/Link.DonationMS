namespace Application.Dtos.UserDTOs;

public class AuthenticationResult
{
    public bool Succeeded { get; set; }
    public string? AccessToken { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? Error { get; set; }
}

public class UserProfileResult
{
    public bool Succeeded { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string>? Roles { get; set; }
    public string? Error { get; set; }
} 