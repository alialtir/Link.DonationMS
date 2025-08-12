using Microsoft.AspNetCore.Mvc;
using Application.Dtos.UserDTOs;
using Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Google.Apis.Auth;
using System.Text.Json;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _configuration;
        public AuthController(IServiceManager serviceManager, IConfiguration configuration)
        {
            _serviceManager = serviceManager;
            _configuration = configuration;  
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromQuery] string clientType = "angular")
        {
            var result = await _serviceManager.AuthenticationService.LoginAsync(dto);
            if (!result.Succeeded)
                return Unauthorized(result);
            if (clientType.Equals("angular", StringComparison.OrdinalIgnoreCase))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                };
                Response.Cookies.Append("AngularAccessToken", result.AccessToken ?? string.Empty, cookieOptions);
            }
            return Ok(result);
        }

       
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var profile = await _serviceManager.AuthenticationService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] string idToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                });
            }
            catch
            {
                return BadRequest("Invalid Google token.");
            }

            var user = await _serviceManager.AuthenticationService.FindOrCreateExternalUserAsync(payload.Email, payload.Name, null);

            // Generate JWT token
            var jwt = await _serviceManager.AuthenticationService.GenerateJwtToken(user);

            return Ok(new { token = jwt });
        }

        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin([FromBody] string accessToken)
        {
            try
            {
                // Validate Facebook access token by calling Facebook Graph API
                using var httpClient = new HttpClient();
                
                // Get user info with email field directly
                var response = await httpClient.GetAsync($"https://graph.facebook.com/me?access_token={accessToken}&fields=id,name,email");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { error = $"Invalid Facebook token. Response: {errorContent}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                
                // Log the response for debugging
                Console.WriteLine($"Facebook API Response: {content}");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var facebookUser = System.Text.Json.JsonSerializer.Deserialize<FacebookUserInfo>(content, options);

                // Check if we got the user data
                if (facebookUser == null)
                {
                    return BadRequest(new { error = "Failed to parse Facebook user data." });
                }

                // If email is not available, try to use the ID as fallback
                var email = facebookUser.Email;
                var name = facebookUser.Name ?? "Facebook User";
                
                if (string.IsNullOrEmpty(email))
                {
                    // Use Facebook ID as email fallback
                    email = $"{facebookUser.Id}@facebook.local";
                }

                var user = await _serviceManager.AuthenticationService.FindOrCreateExternalUserAsync(email, name, null);

                // Generate JWT token
                var jwt = await _serviceManager.AuthenticationService.GenerateJwtToken(user);

                return Ok(new { token = jwt });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Facebook login failed: {ex.Message}" });
            }
        }


    }

    // Helper class for Facebook user info
    public class FacebookUserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
} 