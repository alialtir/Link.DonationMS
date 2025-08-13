using Application.Dtos.UserDTOs;
using Domain.Models;

namespace Application.Services.Abstractions;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(LoginDto loginDto);

    Task<User> FindOrCreateExternalUserAsync(string email, string displayNameEn, string displayNameAr = null);
    Task<string> GenerateJwtToken(User user);
} 