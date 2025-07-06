using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Domain.Models;
using Application.Dtos.UserDTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class AuthenticationService : Application.Services.Abstractions.IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthenticationResult> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Succeeded = false,
                    Error = "Invalid credentials"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                var identity = new ClaimsIdentity(
                    authenticationType: "Bearer",
                    nameType: ClaimTypes.Name,
                    roleType: ClaimTypes.Role);

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));
                
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                var principal = new ClaimsPrincipal(identity);
                
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    await httpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
                    
                    return new AuthenticationResult
                    {
                        Succeeded = true,
                        UserId = user.Id.ToString(),
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles.ToList()
                    };
                }
            }

            return new AuthenticationResult
            {
                Succeeded = false,
                Error = "Invalid credentials"
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Error = ex.Message
            };
        }
    }

    public async Task<UserProfileResult> GetUserProfileAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserProfileResult
                {
                    Succeeded = false,
                    Error = "User not found"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            
            return new UserProfileResult
            {
                Succeeded = true,
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };
        }
        catch (Exception ex)
        {
            return new UserProfileResult
            {
                Succeeded = false,
                Error = ex.Message
            };
        }
    }
} 