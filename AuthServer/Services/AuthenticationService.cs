//using Application.Dtos.UserDTOs;
//using Application.Services.Abstractions;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using System.Security.Claims;
//using Domain.Models;
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//namespace AuthServer.Services
//{
//    public class AuthenticationService : IAuthenticationService
//    {
//        private readonly UserManager<User> _userManager;
//        private readonly IConfiguration _configuration;

//        public AuthenticationService(
//            UserManager<User> userManager,
//            IConfiguration configuration)
//        {
//            _userManager = userManager;
//            _configuration = configuration;
//        }

//        public async Task<AuthenticationResult> LoginAsync(LoginDto loginDto)
//        {
//            var user = await _userManager.FindByEmailAsync(loginDto.UserName) 
//                ?? await _userManager.FindByNameAsync(loginDto.UserName);

//            if (user == null)
//            {
//                return new AuthenticationResult
//                {
//                    Succeeded = false,
//                    Error = "Invalid username or password"
//                };
//            }

//            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
//            if (!isValidPassword)
//            {
//                return new AuthenticationResult
//                {
//                    Succeeded = false,
//                    Error = "Invalid username or password"
//                };
//            }

//            var roles = await _userManager.GetRolesAsync(user);
//            var accessToken = GenerateJwtTokenAsync(user, roles);

//            return new AuthenticationResult
//            {
//                Succeeded = true,
//                AccessToken = accessToken,
//                UserId = user.Id.ToString(),
//                UserName = user.UserName,
//                Email = user.Email,
//                Roles = roles.ToList()
//            };
//        }

//        public async Task<UserProfileResult> GetUserProfileAsync(string userId)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//            {
//                return new UserProfileResult
//                {
//                    Succeeded = false,
//                    Error = "User not found"
//                };
//            }

//            var roles = await _userManager.GetRolesAsync(user);

//            return new UserProfileResult
//            {
//                Succeeded = true,
//                UserId = user.Id.ToString(),
//                UserName = user.UserName,
//                Email = user.Email,
//                //FirstName = user.DisplayName,
//                //LastName = "",
//                Roles = roles.ToList()
//            };
//        }

//        private string GenerateJwtTokenAsync(User user, IList<string> roles)
//        {
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-super-secret-key-with-at-least-256-bits"));
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.UserName ?? ""),
//                new Claim(ClaimTypes.Email, user.Email ?? ""),
//                new Claim("display_name", user.DisplayName ?? "")
//            };

//            foreach (var role in roles)
//            {
//                claims.Add(new Claim(ClaimTypes.Role, role));
//            }

//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"] ?? "https://localhost:5001",
//                audience: _configuration["Jwt:Audience"] ?? "https://localhost:5001",
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(24),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//} 