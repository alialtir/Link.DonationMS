using Application.Dtos.UserDTOs;

namespace Application.Services.Abstractions;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(LoginDto loginDto);
    Task<UserProfileResult> GetUserProfileAsync(string userId);
} 