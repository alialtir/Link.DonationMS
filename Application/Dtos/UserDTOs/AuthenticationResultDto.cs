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
    public bool RequiresPasswordReset { get; set; }
}

public class UserProfileResult
{
    public bool Succeeded { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public List<string>? Roles { get; set; }
    public string? Error { get; set; }
} 