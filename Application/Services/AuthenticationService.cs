using Application.Dtos.UserDTOs;
using Application.Services.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new AuthenticationResult { Succeeded = false, Error = "Invalid credentials" };

            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtTokenAsync(user, roles);
            return new AuthenticationResult
            {
                Succeeded = true,
                AccessToken = token,
                Roles = roles.ToList(),
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<UserProfileResult> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            var names = (user.DisplayName ?? "").Split(' ', 2);
            return new UserProfileResult
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Roles = roles.ToList(),
                Succeeded = true,
                Error = null
            };
        }

        private async Task<string> GenerateJwtTokenAsync(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtOptions:SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["jwtOptions:Issuer"],
                audience: _configuration["jwtOptions:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["jwtOptions:DurationInDays"])),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 