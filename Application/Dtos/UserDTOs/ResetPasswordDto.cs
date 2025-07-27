namespace DTOs.UserDTOs;

public class ResetPasswordDto
{
    public string UserId { get; set; }
    public string NewPassword { get; set; }
}

public class ResetPasswordByEmailDto
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
} 